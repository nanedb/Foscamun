using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Foscamun.Data;
using Foscamun.Models;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Data;

namespace Foscamun.ViewModels
{
    /// <summary>
    /// ViewModel for managing an ICJ session.
    /// Handles speakers list, warnings for advocates/jurors, and voting navigation.
    /// </summary>
    public partial class ICJSessionViewModel : ObservableObject
    {
        private readonly SqliteDataAccess _dataAccess;
        private readonly List<ICJRollCallMember> _presentMembers; // Immutable list of present members
        private readonly Action<List<ICJRollCallMember>> _navigateToVoting;

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

        public string CommitteeLogoPath
        {
            get
            {
                string logoPath = Path.Combine(SqliteDataAccess.CommitteeLogoFolder, "ICJ.svg");
                if (File.Exists(logoPath))
                {
                    return logoPath;
                }
                return Path.Combine(SqliteDataAccess.CommitteeLogoFolder, "Generic.svg");
            }
        }

        public IRelayCommand<ICJRollCallMember> AddSpeakerCommand { get; }
        public IRelayCommand RemoveCurrentSpeakerCommand { get; }
        public IRelayCommand WarnCurrentSpeakerCommand { get; }
        public IRelayCommand<ICJRollCallMember> RemoveWarningCommand { get; }
        public IRelayCommand OpenTimerCommand { get; }
        public IRelayCommand OpenVotingCommand { get; }

        public ICJSessionViewModel(string judge, string viceJudge1, string viceJudge2, string topic, int session, List<ICJRollCallMember> presentMembers, Action<List<ICJRollCallMember>> navigateToVoting, SqliteDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
            _presentMembers = presentMembers;
            _navigateToVoting = navigateToVoting;

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

            // Custom sorting: first by role (Defense, Plaintiff, Juror), then by last name
            var viewAvailable = (ListCollectionView)CollectionViewSource.GetDefaultView(AvailableSpeakers);
            viewAvailable.SortDescriptions.Clear();
            viewAvailable.CustomSort = new ICJMemberComparer();

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
                if (!string.IsNullOrEmpty(CurrentSpeaker.Member.Name) && 
                    !string.IsNullOrEmpty(CurrentSpeaker.Member.Kind))
                {
                    await _dataAccess.UpdateICJMemberWarningsAsync(
                        CurrentSpeaker.Member.Name, 
                        CurrentSpeaker.Member.Kind, 
                        CurrentSpeaker.Warnings);
                }

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
                    if (!string.IsNullOrEmpty(member.Member.Name) && 
                        !string.IsNullOrEmpty(member.Member.Kind))
                    {
                        await _dataAccess.UpdateICJMemberWarningsAsync(
                            member.Member.Name, 
                            member.Member.Kind, 
                            member.Warnings);
                    }
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
            // Solo i Jurors possono votare nell'ICJ
            var voters = _presentMembers
                .Where(m => m.Member.Kind != null && m.Member.Kind.Equals("juror", StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (voters.Count == 0)
            {
                MessageBox.Show(
                    Properties.Settings.Default.Lang switch
                    {
                        "fr" => "Aucun juré disponible pour voter.",
                        "es" => "No hay jurados disponibles para votar.",
                        _ => "No jurors available to vote."
                    },
                    Properties.Settings.Default.Lang switch
                    {
                        "fr" => "Aucun juré",
                        "es" => "Sin jurados",
                        _ => "No Jurors"
                    },
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            // Navigate to ICJ-specific voting page
            _navigateToVoting(voters);
        }
    }
}

