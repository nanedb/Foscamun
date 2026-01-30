using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Foscamun2026.Data;
using Foscamun2026.Models;
using Foscamun2026.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

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

        public bool IsEditCommitteeEnabled => SelectedCommittee != null;

        public SetupPageViewModel(SqliteDataAccess db)
        {
            _db = db;
            _ = LoadCommitteesAsync();
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

                // ⭐ AGGIUNGIAMO ICJ MANUALMENTE
                Committees.Add(new Committee
                {
                    CommID = -1,          // valore speciale per ICJ
                    Name = "ICJ",
                    TopicA = "",
                    TopicB = "",
                    President = "",
                    VicePresident = "",
                    Moderator = ""
                });

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

            if (value != null)
            {
                Properties.Settings.Default.SelCommName = value.Name;
                Properties.Settings.Default.Save();
            }
        }

        // -------------------------
        //  ADD
        // -------------------------

        [RelayCommand]
        private void AddCommittee()
        {
            var page = new AddCommitteePage(_db);
            MainWindow.Instance.NavigateRightFrame(page);
        }

        // -------------------------
        //  EDIT
        // -------------------------

        [RelayCommand]
        private void EditCommittee()
        {
            if (SelectedCommittee == null)
            {
                MessageBox.Show("Please select a committee to edit.");
                return;
            }

            // ⭐ BLOCCA ICJ
            if (SelectedCommittee.CommID == -1)
            {
                MessageBox.Show("Use the 'Edit ICJ' button to modify the ICJ.");
                return;
            }

            var page = new AddCommitteePage(_db, SelectedCommittee);
            MainWindow.Instance.NavigateRightFrame(page);
        }

        // -------------------------
        //  EDIT ICJ  ⭐ NUOVO
        // -------------------------

        [RelayCommand]
        private void EditICJ()
        {
            OpenICJSetup();
        }

        private void OpenICJSetup()
        {
            var page = new AddICJPage(_db);
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

            Committees.Remove(SelectedCommittee);
            SelectedCommittee = null;
        }
    }
}