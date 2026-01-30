using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Foscamun2026.Data;
using Foscamun2026.Models;
using Foscamun2026.Views;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Foscamun2026.ViewModels
{
    public partial class AddICJViewModel : ObservableObject
    {
        private readonly SqliteDataAccess _db;

        // -------------------------
        //  PROPRIETÀ ICJ
        // -------------------------

        [ObservableProperty]
        private string topic = "";

        [ObservableProperty]
        private string president = "";

        [ObservableProperty]
        private string moderator = "";

        // -------------------------
        //  PAESI
        // -------------------------

        public ObservableCollection<Country> AvailableCountries { get; } = new();
        public ObservableCollection<Country> SelectedCountries { get; } = new();

        [ObservableProperty]
        private Country? selectedCountry;

        // -------------------------
        //  MEMBRI ICJ
        // -------------------------

        public ObservableCollection<ICJMember> Members { get; } = new();

        [ObservableProperty]
        private ICJMember? selectedMember;

        // -------------------------
        //  COSTRUTTORE
        // -------------------------

        public AddICJViewModel(SqliteDataAccess db)
        {
            _db = db;

            _ = LoadICJAsync();
            _ = LoadCountriesAsync();
            _ = LoadMembersAsync();
        }

        // -------------------------
        //  CARICAMENTO DATI ICJ
        // -------------------------

        private async Task LoadICJAsync()
        {
            var data = await _db.LoadICJAsync();

            Topic = data.Topic;
            President = data.President;
            Moderator = data.Moderator;
        }

        // -------------------------
        //  CARICAMENTO PAESI
        // -------------------------

        private async Task LoadCountriesAsync()
        {
            var all = await _db.LoadAllCountriesAsync();
            var assigned = await _db.LoadICJCountriesAsync();

            AvailableCountries.Clear();
            SelectedCountries.Clear();

            foreach (var c in all)
            {
                if (assigned.Any(a => a.IsoCode == c.IsoCode))
                    SelectedCountries.Add(c);
                else
                    AvailableCountries.Add(c);
            }
        }

        // -------------------------
        //  CARICAMENTO MEMBRI
        // -------------------------

        private async Task LoadMembersAsync()
        {
            var list = await _db.LoadICJMembersAsync();

            Members.Clear();
            foreach (var m in list)
                Members.Add(m);
        }

        // -------------------------
        //  GESTIONE PAESI
        // -------------------------

        public void AddCountry(Country c)
        {
            AvailableCountries.Remove(c);
            SelectedCountries.Add(c);
        }

        public void RemoveCountry(Country c)
        {
            SelectedCountries.Remove(c);
            AvailableCountries.Add(c);
        }

        // -------------------------
        //  COMANDI
        // -------------------------

        [RelayCommand]
        private void AddMember()
        {
            //MainWindow.Instance.NavigateRightFrame(new AddICJMemberPage(_db));
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            await _db.SaveICJAsync(Topic, President, Moderator);
            await _db.SaveICJCountriesAsync(SelectedCountries);

            MainWindow.Instance.NavigateRightFrame(new SetupPage());
        }

        [RelayCommand]
        private void Cancel()
        {
            MainWindow.Instance.NavigateRightFrame(new SetupPage());
        }
    }
}