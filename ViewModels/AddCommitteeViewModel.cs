using Foscamun2026.Data;
using Foscamun2026.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foscamun2026.ViewModels
{
    public class AddCommitteeViewModel
    {
        private readonly SqliteDataAccess _db;

        public List<Country> AllCountries { get; private set; }
        public List<Country> SelectedCountries { get; private set; }
        public Committee Committee { get; set; }

        public AddCommitteeViewModel(SqliteDataAccess db)
        {
            _db = db;

            AllCountries = new List<Country>();
            SelectedCountries = new List<Country>();
            Committee = new Committee();

            _ = LoadCountriesAsync();
        }

        private async Task LoadCountriesAsync()
        {
            AllCountries = await _db.LoadAllCountriesAsync();
        }

        public void AllCountriesClicked(Country country)
        {
            SelectedCountries.Add(country);
            AllCountries.Remove(country);

            AllCountries.Sort((x, y) => x.Name.CompareTo(y.Name));
            SelectedCountries.Sort((x, y) => x.Name.CompareTo(y.Name));
        }

        public void SelectedCountriesClicked(Country country)
        {
            AllCountries.Add(country);
            SelectedCountries.Remove(country);

            SelectedCountries.Sort((x, y) => x.Name.CompareTo(y.Name));
            AllCountries.Sort((x, y) => x.Name.CompareTo(y.Name));
        }

        //public async Task AddRowAsync()
        //{
        //    Committee newComm = await _db.AddCommitteeAsync(Committee);

        //    foreach (var item in SelectedCountries)
        //    {
        //        await _db.InsertSelectedCountryAsync(newComm.CommID, item.IsoCode);
        //    }

        //    Properties.Settings.Default.SelCommID = newComm.CommID;
        //    Properties.Settings.Default.Save();
        //}
    }
}