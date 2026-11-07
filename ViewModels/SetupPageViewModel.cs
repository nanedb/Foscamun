using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Foscamun.Data;
using Foscamun.Models;
using Foscamun.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace Foscamun.ViewModels
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

        // Edit ICJ: only with ICJ selected
        public bool IsICJButtonEnabled => SelectedCommittee?.CommID == -1;

        // Add ICJ: only while ICJ is missing from the list
        public bool IsAddICJButtonEnabled => Committees != null && !Committees.Any(c => c.CommID == -1);

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
                // Committees without a logo get the generic one; say so once the page is shown
                var missing = SqliteDataAccess.EnsureCommitteeLogos(list.Select(c => c.Name));
                if (missing.Count > 0)
                    Application.Current.Dispatcher.BeginInvoke(() => ShowMissingLogos(missing), DispatcherPriority.ApplicationIdle);
                Committees = new ObservableCollection<Committee>(list);

                var savedName = Properties.Settings.Default.SelCommName;

                if (!string.IsNullOrWhiteSpace(savedName))
                {
                    SelectedCommittee = Committees.FirstOrDefault(c => c.Name == savedName);

                    // The saved committee no longer exists: forget it
                    if (SelectedCommittee == null)
                        ClearSavedCommittee();
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
            OnPropertyChanged(nameof(IsICJButtonEnabled));

            if (value != null)
            {
                Properties.Settings.Default.SelCommName = value.Name;
                Properties.Settings.Default.Save();
            }
        }

        partial void OnCommitteesChanged(ObservableCollection<Committee> value)
        {
            OnPropertyChanged(nameof(IsICJButtonEnabled));
            OnPropertyChanged(nameof(IsAddICJButtonEnabled));
        }

        // -------------------------
        //  ADD COMMITTEE
        // -------------------------

        [RelayCommand]
        private void AddCommittee()
        {
            var page = new CommitteeEditPage(_db);
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
            var vm = new ICJEditViewModel(icj, countries, _db.ICJRepository, MainWindow.Instance);

            // Crea la pagina e assegna il DataContext
            var page = new ICJEditPage
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
                AppDialog.Show(AppDialog.Text("SelectToEditMsg"));
                return;
            }

            // Apre CommitteeEditPage popolata con i dati del comitato selezionato
            var page = new CommitteeEditPage(_db, SelectedCommittee);
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
                AppDialog.Show(AppDialog.Text("SelectToDeleteMsg"));
                return;
            }

            var result = AppDialog.Show(
                AppDialog.Text("ConfirmDeleteMsg", SelectedCommittee.Name),
                AppDialog.Text("ConfirmDeleteTitle"),
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            var removed = SelectedCommittee;
            await _db.RemoveCommitteeAsync(removed);

            // If ICJ was deleted, reload the entire list (because ICJ is dynamically added)
            if (SelectedCommittee.CommID == -1)
            {
                await LoadCommitteesAsync();
            }
            else
            {
                Committees.Remove(SelectedCommittee);
            }

            if (removed.Name == Properties.Settings.Default.SelCommName)
                ClearSavedCommittee();

            SelectedCommittee = null;
        }

        private static void ShowMissingLogos(List<string> names)
        {
            var app = Application.Current;
            string message = string.Format((string)app.FindResource("LogoMissingMsg"),
                string.Join(", ", names), SqliteDataAccess.CommitteeLogoFolder, names[0]);
            AppDialog.Show(message, (string)app.FindResource("LogoMissingTitle"),
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private static void ClearSavedCommittee()
        {
            Properties.Settings.Default.SelCommName = "";
            Properties.Settings.Default.Save();
        }
    }
}
