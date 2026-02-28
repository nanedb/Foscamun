using CommunityToolkit.Mvvm.Input;
using Foscamun.Data;
using Foscamun.Models;
using Foscamun.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Foscamun.ViewModels
{
    /// <summary>
    /// ViewModel for creating and editing committees.
    /// Supports both Add (new committee) and Edit (existing committee) modes.
    /// </summary>
    public class CommitteeEditViewModel : INotifyPropertyChanged
    {
        private readonly SqliteDataAccess _db;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        /// <summary>
        /// Indicates whether the ViewModel is in Edit mode (true) or Add mode (false).
        /// </summary>
        public bool IsEditMode { get; private set; }

        public Country? SelectedCountry { get; set; }

        public ObservableCollection<Country> AllCountries { get; } = new();
        public ObservableCollection<Country> SelectedCountries { get; } = new();

        public Committee Committee { get; set; }

        private bool _canSave;
        /// <summary>
        /// Determines if the Save button should be enabled based on validation.
        /// </summary>
        public bool CanSave
        {
            get => _canSave;
            set
            {
                if (_canSave == value)
                    return;

                _canSave = value;
                OnPropertyChanged();
                SaveCommand.NotifyCanExecuteChanged();
            }
        }

        public IRelayCommand SaveCommand { get; private set; } = null!;
        public IRelayCommand CancelCommand { get; private set; } = null!;

        /// <summary>
        /// Constructor for Add mode - creates a new committee.
        /// </summary>
        public CommitteeEditViewModel(SqliteDataAccess db)
        {
            _db = db;
            Committee = new Committee();
            IsEditMode = false;

            HookEventsAndCommands();
            _ = LoadCountriesAsync();
        }

        /// <summary>
        /// Constructor for Edit mode - edits an existing committee.
        /// </summary>
        public CommitteeEditViewModel(SqliteDataAccess db, Committee committeeToEdit)
        {
            _db = db;
            Committee = committeeToEdit;
            IsEditMode = true;

            HookEventsAndCommands();
            _ = LoadCountriesForEditAsync();
        }

        /// <summary>
        /// Sets up event handlers, commands, and sorting for country lists.
        /// </summary>
        private void HookEventsAndCommands()
        {
            // Enable sorting by country name for both lists
            var viewAll = CollectionViewSource.GetDefaultView(AllCountries);
            viewAll.SortDescriptions.Add(
                new SortDescription(nameof(Country.Name), ListSortDirection.Ascending));

            var viewSel = CollectionViewSource.GetDefaultView(SelectedCountries);
            viewSel.SortDescriptions.Add(
                new SortDescription(nameof(Country.Name), ListSortDirection.Ascending));

            // Refresh country names when language changes
            App.LanguageChanged += OnLanguageChanged;

            SaveCommand = new RelayCommand(SaveCommittee, () => CanSave);
            CancelCommand = new RelayCommand(Cancel);

            // Validate whenever committee properties change
            Committee.PropertyChanged += (_, __) => Validate();
        }

        private void OnLanguageChanged()
        {
            CollectionViewSource.GetDefaultView(AllCountries).Refresh();
            CollectionViewSource.GetDefaultView(SelectedCountries).Refresh();
        }

        /// <summary>
        /// Loads all countries for Add mode (new committee).
        /// </summary>
        private async Task LoadCountriesAsync()
        {
            try
            {
                var countries = await _db.LoadAllCountriesAsync();

                AllCountries.Clear();
                foreach (var c in countries)
                    AllCountries.Add(c);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR: " + ex.Message);
            }
        }

        /// <summary>
        /// Loads countries for Edit mode, splitting them into selected and available lists.
        /// </summary>
        private async Task LoadCountriesForEditAsync()
        {
            try
            {
                var all = await _db.LoadAllCountriesAsync();
                var selected = await _db.LoadCountriesForCommitteeAsync(Committee.CommID);

                AllCountries.Clear();
                SelectedCountries.Clear();

                // Split countries into selected (already in committee) and available (not in committee)
                foreach (var c in all)
                {
                    if (selected.Any(s => s.IsoCode == c.IsoCode))
                        SelectedCountries.Add(c);
                    else
                        AllCountries.Add(c);
                }

                Validate();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR: " + ex.Message);
            }
        }

        /// <summary>
        /// Moves a country from available list to selected list.
        /// </summary>
        public void AllCountriesClicked(Country country)
        {
            AllCountries.Remove(country);
            SelectedCountries.Add(country);
            Validate();
        }

        /// <summary>
        /// Moves a country from selected list back to available list.
        /// </summary>
        public void SelectedCountriesClicked(Country country)
        {
            SelectedCountries.Remove(country);
            AllCountries.Add(country);
            Validate();
        }

        /// <summary>
        /// Validates that all required fields are filled and at least one country is selected.
        /// </summary>
        private void Validate()
        {
            CanSave =
                !string.IsNullOrWhiteSpace(Committee.Name) &&
                !string.IsNullOrWhiteSpace(Committee.Topic) &&
                !string.IsNullOrWhiteSpace(Committee.President) &&
                !string.IsNullOrWhiteSpace(Committee.VicePresident) &&
                !string.IsNullOrWhiteSpace(Committee.Moderator) &&
                SelectedCountries.Count > 0;
        }

        /// <summary>
        /// Saves the committee (creates new or updates existing) and associated countries.
        /// </summary>
        private async void SaveCommittee()
        {
            if (!CanSave)
            {
                MessageBox.Show("Please fill all fields and select at least one country.");
                return;
            }

            if (IsEditMode)
            {
                // Update existing committee and replace country associations
                await _db.UpdateCommitteeAsync(Committee);
                await _db.DeleteCountriesForCommitteeAsync(Committee.CommID);
            }
            else
            {
                // Create new committee
                await _db.AddCommitteeAsync(Committee);
            }

            // Insert all selected countries for this committee
            foreach (var c in SelectedCountries)
                await _db.InsertSelectedCountryAsync(Committee.CommID, c.IsoCode);

            MessageBox.Show(IsEditMode ? "Committee updated." : "Committee created.");

            NavigateBack();
        }

        private void Cancel()
        {
            NavigateBack();
        }

        private void NavigateBack()
        {
            MainWindow.Instance.NavigateRightFrame(new SetupPage());
        }
    }
}
