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

            //Debug.WriteLine("VM: ctor chiamato");

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
                //Debug.WriteLine($"VM: GetCommitteesAsync ha restituito {list.Count} comitati");

                Committees = new ObservableCollection<Committee>(list);

                var savedName = Properties.Settings.Default.SelCommName;
                //Debug.WriteLine($"VM: SelCommName = '{savedName}'");

                if (!string.IsNullOrWhiteSpace(savedName))
                {
                    SelectedCommittee = Committees.FirstOrDefault(c => c.Name == savedName);
                    //Debug.WriteLine($"VM: SelectedCommittee = '{SelectedCommittee?.Name}'");
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
            var vm = new AddCommitteeViewModel(_db);
            var page = new AddCommitteePage { DataContext = vm };

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

            var vm = new AddCommitteeViewModel(_db, SelectedCommittee);
            var page = new AddCommitteePage { DataContext = vm };

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