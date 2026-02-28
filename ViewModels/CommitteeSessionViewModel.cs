using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Foscamun.Data;
using Foscamun.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Data;

namespace Foscamun.ViewModels
{
    /// <summary>
    /// ViewModel for managing a committee session.
    /// Handles speakers list, warnings, and voting navigation.
    /// </summary>
    public partial class CommitteeSessionViewModel : ObservableObject
    {
        private readonly Committee _committee;
        private readonly Action<List<Country>> _navigateToVoting;
        private readonly SqliteDataAccess _dataAccess;
        private readonly List<Country> _presentCountries; // Immutable list of present countries

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

        public string CommitteeLogoPath
        {
            get
            {
                string logoPath = Path.Combine(SqliteDataAccess.CommitteeLogoFolder, $"{_committee.Name}.svg");
                if (File.Exists(logoPath))
                {
                    return logoPath;
                }
                return Path.Combine(SqliteDataAccess.CommitteeLogoFolder, "Generic.svg");
            }
        }

        public IRelayCommand<Country> AddSpeakerCommand { get; }
        public IRelayCommand RemoveCurrentSpeakerCommand { get; }
        public IRelayCommand WarnCurrentSpeakerCommand { get; }
        public IRelayCommand<Country> RemoveWarningCommand { get; }
        public IRelayCommand OpenTimerCommand { get; }
        public IRelayCommand OpenVotingCommand { get; }

        public CommitteeSessionViewModel(Committee committee, string topic, int session, List<Country> presentCountries, Action<List<Country>> navigateToVoting, SqliteDataAccess dataAccess)
        {
            _committee = committee;
            _navigateToVoting = navigateToVoting;
            _dataAccess = dataAccess;
            _presentCountries = presentCountries;

            CommitteeName = committee.Name;
            President = committee.President;
            VicePresident = committee.VicePresident;
            Moderator = committee.Moderator;
            Topic = topic;
            Session = session;

            // Add present countries to available speakers list
            foreach (var country in presentCountries.OrderBy(c => c.Name))
            {
                AvailableSpeakers.Add(country);
            }

            // Enable sorting by country name
            var viewAvailable = CollectionViewSource.GetDefaultView(AvailableSpeakers);
            viewAvailable.SortDescriptions.Add(
                new SortDescription(nameof(Country.Name), ListSortDirection.Ascending));

            // Refresh country names when language changes
            App.LanguageChanged += OnLanguageChanged;

            AddSpeakerCommand = new RelayCommand<Country>(AddSpeaker);
            RemoveCurrentSpeakerCommand = new RelayCommand(RemoveCurrentSpeaker, CanRemoveCurrentSpeaker);
            WarnCurrentSpeakerCommand = new RelayCommand(WarnCurrentSpeaker, CanWarnCurrentSpeaker);
            RemoveWarningCommand = new RelayCommand<Country>(RemoveWarning);
            OpenTimerCommand = new RelayCommand(OpenTimer);
            OpenVotingCommand = new RelayCommand(OpenVoting);

            // Load existing warnings from database
            _ = LoadWarningsAsync();
        }

        /// <summary>
        /// Loads countries with existing warnings from the database.
        /// </summary>
        private async Task LoadWarningsAsync()
        {
            try
            {
                var warnedCountries = await _dataAccess.LoadCountriesWithWarningsAsync(_committee.CommID);

                foreach (var country in warnedCountries)
                {
                    // Find if country is in available speakers to use the same instance
                    var existingCountry = AvailableSpeakers.FirstOrDefault(c => c.IsoCode == country.IsoCode);
                    if (existingCountry != null)
                    {
                        existingCountry.Warnings = country.Warnings;
                        WarnedList.Add(existingCountry);
                    }
                    else
                    {
                        // Country is not present but has warnings, add anyway
                        WarnedList.Add(country);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR loading warnings: {ex.Message}");
            }
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

        private async void WarnCurrentSpeaker()
        {
            if (CurrentSpeaker != null)
            {
                CurrentSpeaker.Warnings++;

                // Update or add to warned list
                var existing = WarnedList.FirstOrDefault(c => c.IsoCode == CurrentSpeaker.IsoCode);
                if (existing != null)
                {
                    WarnedList.Remove(existing);
                }
                WarnedList.Add(CurrentSpeaker);

                // Save to database
                await _dataAccess.UpdateCountryWarningsAsync(_committee.CommID, CurrentSpeaker.IsoCode, CurrentSpeaker.Warnings);

                CollectionViewSource.GetDefaultView(WarnedList).Refresh();
            }
        }

        private async void RemoveWarning(Country? country)
        {
            if (country != null)
            {
                if (country.Warnings > 0)
                {
                    country.Warnings--;

                    // Update in database
                    await _dataAccess.UpdateCountryWarningsAsync(_committee.CommID, country.IsoCode, country.Warnings);
                }

                if (country.Warnings == 0)
                {
                    WarnedList.Remove(country);
                }

                CollectionViewSource.GetDefaultView(WarnedList).Refresh();
            }
        }

        private void OpenTimer()
        {
            var timer = new Views.TimerWindow();
            timer.Owner = Application.Current.MainWindow;
            timer.Show();
        }

        /// <summary>
        /// Opens voting page with the original list of present countries.
        /// All present countries can vote, regardless of speakers list.
        /// </summary>
        private void OpenVoting()
        {
            var voters = _presentCountries;

            if (voters.Count == 0)
            {
                System.Windows.MessageBox.Show(
                    Properties.Settings.Default.Lang switch
                    {
                        "fr" => "Aucun pays disponible pour voter.",
                        "es" => "No hay países disponibles para votar.",
                        _ => "No countries available to vote."
                    },
                    Properties.Settings.Default.Lang switch
                    {
                        "fr" => "Aucun votant",
                        "es" => "Sin votantes",
                        _ => "No Voters"
                    },
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Information);
                return;
            }

            _navigateToVoting(voters);
        }
    }
}
