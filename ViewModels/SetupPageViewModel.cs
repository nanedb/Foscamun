using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Foscamun2026.Data;
using Foscamun2026.Models;
using Foscamun2026.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Foscamun2026.ViewModels
{
    public partial class SetupPageViewModel : ObservableObject
    {
        private readonly SqliteDataAccess _db;

        // -------------------------
        //  PROPRIETÀ
        // -------------------------

        [ObservableProperty]
        private ObservableCollection<Committee> committees = new();

        [ObservableProperty]
        private Committee? selectedCommittee;

        public bool IsEditCommitteeEnabled => SelectedCommittee != null && SelectedCommittee.Name != "ICJ";

        public bool IsRemoveCommitteeEnabled => SelectedCommittee != null;

        public bool IsICJButtonEnabled => true; // Always enabled to allow creating or editing ICJ

        public SetupPageViewModel(SqliteDataAccess db)
        {
            _db = db;
            _ = LoadCommitteesAsync();

            // Applica ordinamento alfabetico alla lista Committees
            var view = CollectionViewSource.GetDefaultView(Committees);
            view.SortDescriptions.Add(new SortDescription(nameof(Committee.Name), ListSortDirection.Ascending));
        }

        // -------------------------
        //  CARICAMENTO COMITATI
        // -------------------------

        private async Task LoadCommitteesAsync()
        {
            Debug.WriteLine("VM: LoadCommitteesAsync chiamato");

            try
            {
                var list = await _db.GetCommitteesAsync();
                Committees = new ObservableCollection<Committee>(list);

                var savedName = Properties.Settings.Default.SelCommName;

                if (!string.IsNullOrWhiteSpace(savedName))
                {
                    SelectedCommittee = Committees.FirstOrDefault(c => c.Name == savedName);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERRORE in LoadCommitteesAsync: " + ex.Message);
            }
        }

        partial void OnSelectedCommitteeChanged(Committee? value)
        {
            OnPropertyChanged(nameof(IsEditCommitteeEnabled));
            OnPropertyChanged(nameof(IsRemoveCommitteeEnabled));

            if (value != null)
            {
                Properties.Settings.Default.SelCommName = value.Name;
                Properties.Settings.Default.Save();
            }
        }

        partial void OnCommitteesChanged(ObservableCollection<Committee> value)
        {
            // No need to update IsICJButtonEnabled anymore as it's always true
        }

        // -------------------------
        //  ADD COMMITTEE
        // -------------------------

        [RelayCommand]
        private void AddCommittee()
        {
            var page = new EditCommitteePage(_db);
            MainWindow.Instance.NavigateRightFrame(page);
        }

        // -------------------------
        //  ICJ BUTTON
        // -------------------------

        [RelayCommand]
        private void OpenICJ()
        {
            // Carica il modello ICJ dal database (se esiste, altrimenti crea uno nuovo vuoto)
            var icj = _db.ICJRepository.Load() ?? new ICJ();

            // Carica tutti i paesi
            var countries = _db.CountryRepository.GetAll();

            // Crea il ViewModel
            var vm = new EditICJViewModel(icj, countries, _db.ICJRepository, MainWindow.Instance);

            // Crea la pagina e assegna il DataContext
            var page = new EditICJPage
            {
                DataContext = vm
            };

            MainWindow.Instance.NavigateRightFrame(page);
        }

        // -------------------------
        //  EDIT COMMITTEE
        // -------------------------

        [RelayCommand]
        private void EditCommittee()
        {
            if (SelectedCommittee == null)
            {
                MessageBox.Show("Please select a committee to edit.");
                return;
            }

            // Apre EditCommitteePage popolata con i dati del comitato selezionato
            var page = new EditCommitteePage(_db, SelectedCommittee);
            MainWindow.Instance.NavigateRightFrame(page);
        }

        // -------------------------
        //  REMOVE
        // -------------------------

        [RelayCommand]
        private async Task RemoveCommitteeAsync()
        {
            if (SelectedCommittee == null)
            {
                MessageBox.Show("Please select a committee to delete.");
                return;
            }

            var result = MessageBox.Show(
                $"Are you sure you want to delete '{SelectedCommittee.Name}'?",
                "Confirm delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            await _db.RemoveCommitteeAsync(SelectedCommittee);

            // If ICJ was deleted, reload the entire list (because ICJ is dynamically added)
            if (SelectedCommittee.CommID == -1)
            {
                await LoadCommitteesAsync();
            }
            else
            {
                Committees.Remove(SelectedCommittee);
            }

            SelectedCommittee = null;
        }
    }
}