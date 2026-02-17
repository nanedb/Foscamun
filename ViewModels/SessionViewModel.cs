using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Foscamun2026.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace Foscamun2026.ViewModels
{
    public partial class SessionViewModel : ObservableObject
    {
        private readonly Committee _committee;

        [ObservableProperty]
        private string committeeName = string.Empty;

        [ObservableProperty]
        private string president = string.Empty;

        [ObservableProperty]
        private string vicePresident = string.Empty;

        [ObservableProperty]
        private string moderator = string.Empty;

        [ObservableProperty]
        private string topic = string.Empty;

        [ObservableProperty]
        private int session;

        [ObservableProperty]
        private Country? selectedSpeaker;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RemoveCurrentSpeakerCommand))]
        [NotifyCanExecuteChangedFor(nameof(WarnCurrentSpeakerCommand))]
        private Country? currentSpeaker;

        [ObservableProperty]
        private Country? selectedWarnedCountry;

        public ObservableCollection<Country> AvailableSpeakers { get; } = new();

        public ObservableCollection<Country> SpeakersList { get; } = new();

        public ObservableCollection<Country> WarnedList { get; } = new();

        public string CommitteeLogoPath => $"pack://application:,,,/Resources/Committee Logo/{_committee.Name}.svg";

        public IRelayCommand<Country> AddSpeakerCommand { get; }
        public IRelayCommand RemoveCurrentSpeakerCommand { get; }
        public IRelayCommand WarnCurrentSpeakerCommand { get; }
        public IRelayCommand<Country> RemoveWarningCommand { get; }
        public IRelayCommand OpenTimerCommand { get; }
        public IRelayCommand OpenVotingCommand { get; }

        public SessionViewModel(Committee committee, string topic, int session, List<Country> presentCountries)
        {
            _committee = committee;

            CommitteeName = committee.Name;
            President = committee.President;
            VicePresident = committee.VicePresident;
            Moderator = committee.Moderator;
            Topic = topic;
            Session = session;

            // Aggiungi i paesi presenti alla lista degli speakers disponibili
            foreach (var country in presentCountries.OrderBy(c => c.Name))
            {
                AvailableSpeakers.Add(country);
            }

            // Sorting per AvailableSpeakers
            var viewAvailable = CollectionViewSource.GetDefaultView(AvailableSpeakers);
            viewAvailable.SortDescriptions.Add(
                new SortDescription(nameof(Country.Name), ListSortDirection.Ascending));

            // Cambio lingua
            App.LanguageChanged += OnLanguageChanged;

            AddSpeakerCommand = new RelayCommand<Country>(AddSpeaker);
            RemoveCurrentSpeakerCommand = new RelayCommand(RemoveCurrentSpeaker, CanRemoveCurrentSpeaker);
            WarnCurrentSpeakerCommand = new RelayCommand(WarnCurrentSpeaker, CanWarnCurrentSpeaker);
            RemoveWarningCommand = new RelayCommand<Country>(RemoveWarning);
            OpenTimerCommand = new RelayCommand(OpenTimer);
            OpenVotingCommand = new RelayCommand(OpenVoting);
        }

        private void OnLanguageChanged()
        {
            CollectionViewSource.GetDefaultView(AvailableSpeakers).Refresh();
        }

        private void AddSpeaker(Country? speaker)
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

        private void WarnCurrentSpeaker()
        {
            if (CurrentSpeaker != null)
            {
                CurrentSpeaker.Warnings++;

                // Aggiorna o aggiungi alla lista warned
                var existing = WarnedList.FirstOrDefault(c => c.IsoCode == CurrentSpeaker.IsoCode);
                if (existing != null)
                {
                    WarnedList.Remove(existing);
                }
                WarnedList.Add(CurrentSpeaker);

                // Refresh della visualizzazione
                CollectionViewSource.GetDefaultView(WarnedList).Refresh();
            }
        }

        private void RemoveWarning(Country? country)
        {
            if (country != null)
            {
                if (country.Warnings > 0)
                {
                    country.Warnings--;
                }

                if (country.Warnings == 0)
                {
                    WarnedList.Remove(country);
                }

                // Refresh della visualizzazione
                CollectionViewSource.GetDefaultView(WarnedList).Refresh();
            }
        }

        private void OpenTimer()
        {
            // TODO: Implementare timer window
            System.Windows.MessageBox.Show("Timer feature coming soon!");
        }

        private void OpenVoting()
        {
            // TODO: Implementare voting window
            System.Windows.MessageBox.Show("Voting feature coming soon!");
        }
    }
}
