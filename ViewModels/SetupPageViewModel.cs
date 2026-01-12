using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Foscamun2026.Data;
using Foscamun2026.Models;
using System.Threading.Tasks;

namespace Foscamun2026.ViewModels
{
    public partial class SetupPageViewModel : ObservableObject
    {
        private readonly SqliteDataAccess _db;

        // LISTA COMITATI
        [ObservableProperty]
        private ObservableCollection<Committee> committees = new();

        // COMITATO SELEZIONATO
        [ObservableProperty]
        private Committee? selectedCommittee;

        // ABILITAZIONE PULSANTI EDIT/REMOVE
        public bool IsEditCommitteeEnabled => SelectedCommittee != null;

        public SetupPageViewModel()
        {
            _db = new SqliteDataAccess();
            _ = LoadCommitteesAsync();
            // Se esiste un comitato salvato, selezionalo
            var savedName = Properties.Settings.Default.SelCommName;
            if (!string.IsNullOrWhiteSpace(savedName))
            {
                SelectedCommittee = Committees?.FirstOrDefault(c => c.Name == savedName);
            }
        }

        private async Task LoadCommitteesAsync()
        {
            try
            {
                var list = await _db.GetCommitteesAsync();

                Committees = new ObservableCollection<Committee>(list);

                // Se esiste un comitato salvato, selezionalo ora
                var savedName = Properties.Settings.Default.SelCommName;
                if (!string.IsNullOrWhiteSpace(savedName))
                {
                    SelectedCommittee = Committees.FirstOrDefault(c => c.Name == savedName);
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
        private async Task AddCommitteeAsync()
        {
            var newComm = new Committee
            {
                Name = "New Committee",
                TopicA = "",
                TopicB = "",
                President = "",
                VicePresident = "",
                Moderator = ""
            };

            SqliteDataAccess.AddCommittee(newComm);
            await LoadCommitteesAsync();
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