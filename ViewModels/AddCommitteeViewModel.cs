using Foscamun2026.Data;
using Foscamun2026.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foscamun2026.ViewModels
{
    public class AddCommitteeViewModel
    {
        public List<Country> AllCountries { get; private set; }

        public List<Country> SelectedCountries { get; private set; }

        public Committee Committee { get; set; }

        public AddCommitteeViewModel()
        {
            AllCountries = SqliteDataAccess.LoadAllCountries();

            SelectedCountries = new List<Country>();

            Committee = new Committee();
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

        public void AddRow()
        {
            Committee NewComm = SqliteDataAccess.AddCommittee(Committee);
            if (SelectedCountries.Count > 0)
            {
                foreach (var item in SelectedCountries)
                {
                    SqliteDataAccess.InsertSelectedCountry(NewComm.CommID, item.IsoCode);
                }
            }
            Properties.Settings.Default.SelCommID = NewComm.CommID;
            Properties.Settings.Default.Save();
        }

    }
}
