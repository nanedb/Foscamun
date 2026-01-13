using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Foscamun2026.Data;
using Foscamun2026.Models;
using Foscamun2026.Views;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Foscamun2026.ViewModels
{
    public partial class SetupPageViewModel : ObservableObject
    {
        private readonly SqliteDataAccess _db;
        public NavigationService? NavigationService { get; set; }

        // LISTA COMITATI
        [ObservableProperty]
        private ObservableCollection<Committee> committees = new();

        // COMITATO SELEZIONATO
        [ObservableProperty]
        private Committee? selectedCommittee;

        // ABILITAZIONE PULSANTI EDIT/REMOVE
        public bool IsEditCommitteeEnabled => SelectedCommittee != null;

        // COSTRUTTORE CORRETTO (DB INIETTATO)
        public SetupPageViewModel(SqliteDataAccess db)
        {
            _db = db;

            System.Diagnostics.Debug.WriteLine("VM: ctor chiamato");

            // Carica i comitati (la selezione avverrà dentro LoadCommitteesAsync)
            _ = LoadCommitteesAsync();
        }

        private async Task LoadCommitteesAsync()
        {
            System.Diagnostics.Debug.WriteLine("VM: LoadCommitteesAsync chiamato");

            try
            {
                var list = await _db.GetCommitteesAsync();
                System.Diagnostics.Debug.WriteLine($"VM: GetCommitteesAsync ha restituito {list.Count} comitati");

                Committees = new ObservableCollection<Committee>(list);

                // Se esiste un comitato salvato, selezionalo ORA (quando la lista è piena)
                var savedName = Properties.Settings.Default.SelCommName;
                System.Diagnostics.Debug.WriteLine($"VM: SelCommName = '{savedName}'");

                if (!string.IsNullOrWhiteSpace(savedName))
                {
                    SelectedCommittee = Committees.FirstOrDefault(c => c.Name == savedName);
                    System.Diagnostics.Debug.WriteLine($"VM: SelectedCommittee = '{SelectedCommittee?.Name}'");

                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERRORE in LoadCommitteesAsync: " + ex.Message);
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

        // AGGIUNTA
        [RelayCommand]
        private void AddCommittee()
        {
            NavigationService?.Navigate(new AddCommitteePage());
        }
        
        // MODIFICA
        [RelayCommand]
        private async Task EditCommitteeAsync()
        {
            if (SelectedCommittee == null)
                return;

            await _db.UpdateCommitteeAsync(SelectedCommittee);
            await LoadCommitteesAsync();
        }

        // RIMOZIONE
        [RelayCommand]
        private async Task RemoveCommitteeAsync()
        {
            if (SelectedCommittee == null)
                return;

            await _db.RemoveCommitteeAsync(SelectedCommittee);
            await LoadCommitteesAsync();
        }
    }
}