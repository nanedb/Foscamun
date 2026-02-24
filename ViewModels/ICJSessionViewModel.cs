using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Foscamun2026.Data;
using Foscamun2026.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace Foscamun2026.ViewModels
{
    public partial class ICJSessionViewModel : ObservableObject
    {
        private readonly SqliteDataAccess _dataAccess;
        private readonly List<ICJRollCallMember> _presentMembers; // Lista immutabile dei presenti

        [ObservableProperty]
        private string judge = string.Empty;

        [ObservableProperty]
        private string viceJudge1 = string.Empty;

        [ObservableProperty]
        private string viceJudge2 = string.Empty;

        [ObservableProperty]
        private string topic = string.Empty;

        [ObservableProperty]
        private int session;

        [ObservableProperty]
        private ICJRollCallMember? selectedSpeaker;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RemoveCurrentSpeakerCommand))]
        [NotifyCanExecuteChangedFor(nameof(WarnCurrentSpeakerCommand))]
        private ICJRollCallMember? currentSpeaker;

        [ObservableProperty]
        private ICJRollCallMember? selectedWarnedMember;

        public ObservableCollection<ICJRollCallMember> AvailableSpeakers { get; } = new();

        public ObservableCollection<ICJRollCallMember> SpeakersList { get; } = new();

        public ObservableCollection<ICJRollCallMember> WarnedList { get; } = new();

        public string CommitteeLogoPath => "pack://application:,,,/Resources/Committee Logo/ICJ.svg";

        public IRelayCommand<ICJRollCallMember> AddSpeakerCommand { get; }
        public IRelayCommand RemoveCurrentSpeakerCommand { get; }
        public IRelayCommand WarnCurrentSpeakerCommand { get; }
        public IRelayCommand<ICJRollCallMember> RemoveWarningCommand { get; }
        public IRelayCommand OpenTimerCommand { get; }
        public IRelayCommand OpenVotingCommand { get; }

        public ICJSessionViewModel(string judge, string viceJudge1, string viceJudge2, string topic, int session, List<ICJRollCallMember> presentMembers, SqliteDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
            _presentMembers = presentMembers;

            this.judge = judge;
            this.viceJudge1 = viceJudge1;
            this.viceJudge2 = viceJudge2;
            this.topic = topic;
            this.session = session;

            // Aggiungi i membri presenti alla lista degli speakers disponibili
            foreach (var member in presentMembers.OrderBy(m => m.DisplayName))
            {
                AvailableSpeakers.Add(member);
            }

            // Sorting per AvailableSpeakers
            var viewAvailable = CollectionViewSource.GetDefaultView(AvailableSpeakers);
            viewAvailable.SortDescriptions.Add(
                new SortDescription(nameof(ICJRollCallMember.DisplayName), ListSortDirection.Ascending));

            AddSpeakerCommand = new RelayCommand<ICJRollCallMember>(AddSpeaker);
            RemoveCurrentSpeakerCommand = new RelayCommand(RemoveCurrentSpeaker, CanRemoveCurrentSpeaker);
            WarnCurrentSpeakerCommand = new RelayCommand(WarnCurrentSpeaker, CanWarnCurrentSpeaker);
            RemoveWarningCommand = new RelayCommand<ICJRollCallMember>(RemoveWarning);
            OpenTimerCommand = new RelayCommand(OpenTimer);
            OpenVotingCommand = new RelayCommand(OpenVoting);

            // Carica warnings esistenti
            _ = LoadWarningsAsync();
        }

        private async Task LoadWarningsAsync()
        {
            try
            {
                var warnedMembers = await _dataAccess.LoadICJMembersWithWarningsAsync();

                foreach (var warnedMember in warnedMembers)
                {
                    // Cerca il membro nella lista presente per usare la stessa istanza
                    var existingMember = AvailableSpeakers.FirstOrDefault(m => 
                        m.Member.Name == warnedMember.Name && m.Member.Kind == warnedMember.Kind);

                    if (existingMember != null)
                    {
                        // Aggiorna i warnings dell'istanza esistente
                        existingMember.Warnings = warnedMember.Warnings;
                        WarnedList.Add(existingMember);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR loading ICJ warnings: {ex.Message}");
            }
        }

        private void AddSpeaker(ICJRollCallMember? speaker)
        {
            if (speaker != null && !SpeakersList.Contains(speaker))
            {
                // Aggiungi alla lista degli speakers
                SpeakersList.Add(speaker);

                // Rimuovi dalla lista degli available speakers
                AvailableSpeakers.Remove(speaker);
            }
        }

        private bool CanRemoveCurrentSpeaker() => CurrentSpeaker != null;

        private void RemoveCurrentSpeaker()
        {
            if (CurrentSpeaker != null)
            {
                var speakerToRemove = CurrentSpeaker;

                // Rimuovi dalla lista degli speakers
                SpeakersList.Remove(speakerToRemove);

                // Riportalo nella lista degli available speakers
                if (!AvailableSpeakers.Contains(speakerToRemove))
                {
                    AvailableSpeakers.Add(speakerToRemove);
                }

                CurrentSpeaker = null;
            }
        }

        private bool CanWarnCurrentSpeaker() => CurrentSpeaker != null;

        private async void WarnCurrentSpeaker()
        {
            if (CurrentSpeaker != null)
            {
                CurrentSpeaker.Warnings++;

                // Aggiorna o aggiungi alla lista warned
                var existing = WarnedList.FirstOrDefault(m => 
                    m.Member.Name == CurrentSpeaker.Member.Name && 
                    m.Member.Kind == CurrentSpeaker.Member.Kind);

                if (existing != null)
                {
                    WarnedList.Remove(existing);
                }
                WarnedList.Add(CurrentSpeaker);

                // Salva nel database
                await _dataAccess.UpdateICJMemberWarningsAsync(
                    CurrentSpeaker.Member.Name, 
                    CurrentSpeaker.Member.Kind, 
                    CurrentSpeaker.Warnings);

                // Refresh della visualizzazione
                CollectionViewSource.GetDefaultView(WarnedList).Refresh();
            }
        }

        private async void RemoveWarning(ICJRollCallMember? member)
        {
            if (member != null)
            {
                if (member.Warnings > 0)
                {
                    member.Warnings--;

                    // Salva nel database
                    await _dataAccess.UpdateICJMemberWarningsAsync(
                        member.Member.Name, 
                        member.Member.Kind, 
                        member.Warnings);
                }

                if (member.Warnings == 0)
                {
                    WarnedList.Remove(member);
                }

                // Refresh della visualizzazione
                CollectionViewSource.GetDefaultView(WarnedList).Refresh();
            }
        }

        private void OpenTimer()
        {
            // Open non-modal timer window
            var timer = new Views.TimerWindow();
            timer.Owner = Application.Current.MainWindow;
            timer.Show();
        }

        private void OpenVoting()
        {
            // TODO: Implement ICJ voting logic (different from committee voting)
            var voters = _presentMembers;
            
            if (voters.Count == 0)
            {
                MessageBox.Show(
                    Properties.Settings.Default.Lang switch
                    {
                        "fr" => "Aucun membre disponible pour voter.",
                        "es" => "No hay miembros disponibles para votar.",
                        _ => "No members available to vote."
                    },
                    Properties.Settings.Default.Lang switch
                    {
                        "fr" => "Aucun votant",
                        "es" => "Sin votantes",
                        _ => "No Voters"
                    },
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            // TODO: Navigate to ICJ-specific voting page
            System.Diagnostics.Debug.WriteLine($"Opening voting for {voters.Count} ICJ members");
        }
    }
}
