using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Foscamun2026.Data;
using Foscamun2026.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Data;

namespace Foscamun2026.ViewModels
{
    public partial class CommitteeRollCallViewModel : ObservableObject
    {
        private readonly SqliteDataAccess _db;
        private readonly Committee _committee;
        private readonly Action<Committee, string, int, List<Country>> _navigateToSession;

        [ObservableProperty]
        private string committeeName = string.Empty;

        [ObservableProperty]
        private string president = string.Empty;

        [ObservableProperty]
        private string vicePresident = string.Empty;

        [ObservableProperty]
        private string moderator = string.Empty;

        [ObservableProperty]
        private string? selectedTopic;

        [ObservableProperty]
        private int selectedSession = 1;

        [ObservableProperty]
        private Country? selectedCountry;

        public ObservableCollection<string> Topics { get; } = new();

        public ObservableCollection<int> Sessions { get; } = new();

        public ObservableCollection<Country> AvailableCountries { get; } = new();

        public ObservableCollection<Country> PresentCountries { get; } = new();

        public string CommitteeLogoPath => $"pack://application:,,,/Resources/Committee Logo/{CommitteeName}.svg";

        public IRelayCommand ProceedCommand { get; }
        public IRelayCommand MarkAllPresentCommand { get; }

        public CommitteeRollCallViewModel(Committee committee, SqliteDataAccess db, Action<Committee, string, int, List<Country>> navigateToSession)
        {
            _committee = committee;
            _db = db;
            _navigateToSession = navigateToSession;

            CommitteeName = committee.Name;
            President = committee.President;
            VicePresident = committee.VicePresident;
            Moderator = committee.Moderator;

            for (int i = 1; i <= 10; i++)
            {
                Sessions.Add(i);
            }

            // Sorting per entrambe le liste
            var viewAvailable = CollectionViewSource.GetDefaultView(AvailableCountries);
            viewAvailable.SortDescriptions.Add(
                new SortDescription(nameof(Country.Name), ListSortDirection.Ascending));

            var viewPresent = CollectionViewSource.GetDefaultView(PresentCountries);
            viewPresent.SortDescriptions.Add(
                new SortDescription(nameof(Country.Name), ListSortDirection.Ascending));

            // Cambio lingua
            App.LanguageChanged += OnLanguageChanged;

            ProceedCommand = new RelayCommand(Proceed, CanProceed);
            MarkAllPresentCommand = new RelayCommand(MarkAllPresent, CanMarkAllPresent);

            _ = LoadCountriesAsync();
        }

        private void OnLanguageChanged()
        {
            CollectionViewSource.GetDefaultView(AvailableCountries).Refresh();
            CollectionViewSource.GetDefaultView(PresentCountries).Refresh();
        }

        private async Task LoadCountriesAsync()
        {
            try
            {
                var countries = await _db.LoadCountriesForCommitteeAsync(_committee.CommID);

                AvailableCountries.Clear();
                foreach (var c in countries)
                    AvailableCountries.Add(c);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR: " + ex.Message);
            }
        }

        public void AvailableCountriesClicked(Country country)
        {
            AvailableCountries.Remove(country);
            PresentCountries.Add(country);
            ProceedCommand.NotifyCanExecuteChanged();
            MarkAllPresentCommand.NotifyCanExecuteChanged();
        }

        public void PresentCountriesClicked(Country country)
        {
            PresentCountries.Remove(country);
            AvailableCountries.Add(country);
            ProceedCommand.NotifyCanExecuteChanged();
            MarkAllPresentCommand.NotifyCanExecuteChanged();
        }

        private bool CanMarkAllPresent()
        {
            return AvailableCountries.Count > 0;
        }

        private void MarkAllPresent()
        {
            var countriesToMove = AvailableCountries.ToList();
            foreach (var country in countriesToMove)
            {
                AvailableCountries.Remove(country);
                PresentCountries.Add(country);
            }
            ProceedCommand.NotifyCanExecuteChanged();
            MarkAllPresentCommand.NotifyCanExecuteChanged();
        }

        private bool CanProceed()
        {
            return PresentCountries.Count > 0;
        }

        private void Proceed()
        {
            var presentCountriesList = PresentCountries.ToList();
            _navigateToSession(_committee, _committee.Topic, SelectedSession, presentCountriesList);
        }
    }
}