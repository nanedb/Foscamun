using CommunityToolkit.Mvvm.Input;
using Foscamun.Data;
using Foscamun.Models;
using System.ComponentModel;
using System.Windows.Data;
using Foscamun.Views;

namespace Foscamun.ViewModels
{
    /// <summary>
    /// ViewModel for a committee session: the speakers are the present countries,
    /// and all of them vote. Lists, clock and warnings live in <see cref="SessionViewModelBase{TSpeaker}"/>.
    /// </summary>
    public partial class CommitteeSessionViewModel : SessionViewModelBase<Country>
    {
        private readonly Committee _committee;
        private readonly Action<List<Country>> _navigateToVoting;
        private readonly SqliteDataAccess _dataAccess;
        private readonly List<Country> _presentCountries; // Immutable list of present countries

        public string CommitteeName { get; }
        public string President { get; }
        public string VicePresident { get; }
        public string Moderator { get; }
        public string Topic { get; }

        public CommitteeSessionViewModel(Committee committee, string topic, int session, List<Country> presentCountries, Action<List<Country>> navigateToVoting, SqliteDataAccess dataAccess)
            : base(session, presentCountries)
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

            // Keep the available list alphabetical as countries come back to it
            CollectionViewSource.GetDefaultView(AvailableSpeakers).SortDescriptions.Add(
                new SortDescription(nameof(Country.Name), ListSortDirection.Ascending));

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
                    // Use the same instance as the available list, so counts stay in sync
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

        protected override Task SaveWarningsAsync(Country country) =>
            _dataAccess.UpdateCountryWarningsAsync(_committee.CommID, country.IsoCode, country.Warnings);

        /// <summary>
        /// Opens voting page with the original list of present countries.
        /// All present countries can vote, regardless of speakers list.
        /// </summary>
        [RelayCommand]
        private void OpenVoting()
        {
            var voters = _presentCountries;

            if (voters.Count == 0)
            {
                AppDialog.Show(
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

            PauseClock();
            _navigateToVoting(voters);
        }
    }
}
