using CommunityToolkit.Mvvm.Input;
using Foscamun2026.Data;
using Foscamun2026.Models;
using Foscamun2026.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Foscamun2026.ViewModels
{
    public class AddCommitteeViewModel : INotifyPropertyChanged
    {
        private readonly SqliteDataAccess _db;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        // -------------------------
        //  PROPRIETÀ
        // -------------------------

        public bool IsEditMode { get; private set; }

        public Country? SelectedCountry { get; set; }

        public ObservableCollection<Country> AllCountries { get; } = new();
        public ObservableCollection<Country> SelectedCountries { get; } = new();

        public Committee Committee { get; set; }

        private bool _canSave;
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

        // -------------------------
        //  COMANDI
        // -------------------------

        public IRelayCommand SaveCommand { get; private set; } = null!;

        public IRelayCommand CancelCommand { get; private set; } = null!;


        // -------------------------
        //  COSTRUTTORI
        // -------------------------

        // Modalità ADD
        public AddCommitteeViewModel(SqliteDataAccess db)
        {
            _db = db;
            Committee = new Committee();
            IsEditMode = false;

            HookEventsAndCommands();
            _ = LoadCountriesAsync();
        }

        // Modalità EDIT
        public AddCommitteeViewModel(SqliteDataAccess db, Committee committeeToEdit)
        {
            _db = db;
            Committee = committeeToEdit;
            IsEditMode = true;

            HookEventsAndCommands();
            _ = LoadCountriesForEditAsync();
        }

        // -------------------------
        //  INIZIALIZZAZIONE COMUNE
        // -------------------------

        private void HookEventsAndCommands()
        {
            // Sorting
            var viewAll = CollectionViewSource.GetDefaultView(AllCountries);
            viewAll.SortDescriptions.Add(
                new SortDescription(nameof(Country.Name), ListSortDirection.Ascending));

            var viewSel = CollectionViewSource.GetDefaultView(SelectedCountries);
            viewSel.SortDescriptions.Add(
                new SortDescription(nameof(Country.Name), ListSortDirection.Ascending));

            // Cambio lingua
            App.LanguageChanged += OnLanguageChanged;

            // Comandi
            SaveCommand = new RelayCommand(SaveCommittee, () => CanSave);
            CancelCommand = new RelayCommand(Cancel);

            // Validazione automatica
            Committee.PropertyChanged += (_, __) => Validate();
        }

        // -------------------------
        //  CAMBIO LINGUA
        // -------------------------

        private void OnLanguageChanged()
        {
            CollectionViewSource.GetDefaultView(AllCountries).Refresh();
            CollectionViewSource.GetDefaultView(SelectedCountries).Refresh();
        }

        // -------------------------
        //  CARICAMENTO PAESI (ADD)
        // -------------------------

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

        // -------------------------
        //  CARICAMENTO PAESI (EDIT)
        // -------------------------

        private async Task LoadCountriesForEditAsync()
        {
            try
            {
                var all = await _db.LoadAllCountriesAsync();
                var selected = await _db.LoadCountriesForCommitteeAsync(Committee.CommID);

                AllCountries.Clear();
                SelectedCountries.Clear();

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

        // -------------------------
        //  CLICK LISTE
        // -------------------------

        public void AllCountriesClicked(Country country)
        {
            AllCountries.Remove(country);
            SelectedCountries.Add(country);
            Validate();
        }

        public void SelectedCountriesClicked(Country country)
        {
            SelectedCountries.Remove(country);
            AllCountries.Add(country);
            Validate();
        }

        // -------------------------
        //  VALIDAZIONE
        // -------------------------

        private void Validate()
        {
            CanSave =
                !string.IsNullOrWhiteSpace(Committee.Name) &&
                !string.IsNullOrWhiteSpace(Committee.TopicA) &&
                !string.IsNullOrWhiteSpace(Committee.TopicB) &&
                !string.IsNullOrWhiteSpace(Committee.President) &&
                !string.IsNullOrWhiteSpace(Committee.VicePresident) &&
                !string.IsNullOrWhiteSpace(Committee.Moderator) &&
                SelectedCountries.Count > 0;
        }

        // -------------------------
        //  SALVATAGGIO
        // -------------------------

        private async void SaveCommittee()
        {
            if (!CanSave)
            {
                MessageBox.Show("Please fill all fields and select at least one country.");
                return;
            }

            if (IsEditMode)
            {
                await _db.UpdateCommitteeAsync(Committee);
                await _db.DeleteCountriesForCommitteeAsync(Committee.CommID);
            }
            else
            {
                await _db.AddCommitteeAsync(Committee);
            }

            foreach (var c in SelectedCountries)
                await _db.InsertSelectedCountryAsync(Committee.CommID, c.IsoCode);

            MessageBox.Show(IsEditMode ? "Committee updated." : "Committee created.");

            NavigateBack();
        }

        // -------------------------
        //  CANCEL
        // -------------------------

        private void Cancel()
        {
            NavigateBack();
        }

        // -------------------------
        //  NAVIGAZIONE
        // -------------------------

        private void NavigateBack()
        {
            MainWindow.Instance.NavigateRightFrame(new SetupPage());
        }
    }
}