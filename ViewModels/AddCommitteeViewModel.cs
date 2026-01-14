using System.Windows.Data;
using System.ComponentModel;
using Foscamun2026.Data;
using Foscamun2026.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Foscamun2026.ViewModels
{
    public class AddCommitteeViewModel
    {
        private readonly SqliteDataAccess _db;

        public Country? SelectedCountry { get; set; }

        public ObservableCollection<Country> AllCountries { get; } = new();

        public ObservableCollection<Country> SelectedCountries { get; } = new();
        public Committee Committee { get; set; }

        public AddCommitteeViewModel(SqliteDataAccess db)
        {
            _db = db;
            Committee = new Committee();

            var viewAll = CollectionViewSource.GetDefaultView(AllCountries);
            viewAll.SortDescriptions.Add(
                new SortDescription(nameof(Country.Name), ListSortDirection.Ascending));

            var viewSel = CollectionViewSource.GetDefaultView(SelectedCountries);
            viewSel.SortDescriptions.Add(
                new SortDescription(nameof(Country.Name), ListSortDirection.Ascending));

            // ascolta il cambio lingua
            App.LanguageChanged += OnLanguageChanged;

            _ = LoadCountriesAsync();
        }

        private void OnLanguageChanged()
        {
            CollectionViewSource.GetDefaultView(AllCountries).Refresh();
            CollectionViewSource.GetDefaultView(SelectedCountries).Refresh();
        }
        private async Task LoadCountriesAsync()
        {
            Debug.WriteLine("LOAD START");

            try
            {
                var countries = await _db.LoadAllCountriesAsync();
                Debug.WriteLine("DB RETURNED: " + countries.Count);

                AllCountries.Clear();
                foreach (var c in countries)
                    AllCountries.Add(c);

                Debug.WriteLine("COUNTRIES LOADED: " + AllCountries.Count);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR: " + ex.Message);
            }
        }

        public void AllCountriesClicked(Country country)
        {
            AllCountries.Remove(country);
            SelectedCountries.Add(country);
        }

        public void SelectedCountriesClicked(Country country)
        {
            SelectedCountries.Remove(country);
            AllCountries.Add(country);
        }
    }
}