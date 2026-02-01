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
        private string _topicA = string.Empty;

        [ObservableProperty]
        private string _topicB = string.Empty;

        [ObservableProperty]
        private string _president = string.Empty;

        [ObservableProperty]
        private string _vicePresident = string.Empty;

        [ObservableProperty]
        private string _moderator = string.Empty;

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

            if (data == null)
                return; // oppure mostra un messaggio di errore

            TopicA = data.TopicA;
            TopicB = data.TopicB;
            President = data.President;
            VicePresident = data.VicePresident;
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

            foreach (var c in all.OrderBy(c => c.Name))
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
        //  GESTIONE PAESI (CLICK SINGOLO)
        // -------------------------

        public void AddCountry(Country c)
        {
            AvailableCountries.Remove(c);
            SelectedCountries.Add(c);
            SortSelectedCountries();
        }

        public void RemoveCountry(Country c)
        {
            SelectedCountries.Remove(c);
            AvailableCountries.Add(c);
            SortAvailableCountries();
        }

        private void SortSelectedCountries()
        {
            var sorted = SelectedCountries.OrderBy(c => c.Name).ToList();
            SelectedCountries.Clear();
            foreach (var c in sorted)
                SelectedCountries.Add(c);
        }

        private void SortAvailableCountries()
        {
            var sorted = AvailableCountries.OrderBy(c => c.Name).ToList();
            AvailableCountries.Clear();
            foreach (var c in sorted)
                AvailableCountries.Add(c);
        }

        // -------------------------
        //  COMANDI
        // -------------------------

        [RelayCommand]
        private void AddMember()
        {
            // 🔥 Devi passare il DB, altrimenti AddICJMemberPage non funziona
            MainWindow.Instance.NavigateRightFrame(new AddICJMemberPage(_db));
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            await _db.SaveICJAsync(new Committee
            {
                Name = "ICJ",
                TopicA = TopicA,
                TopicB = TopicB,
                President = President,
                VicePresident = VicePresident,
                Moderator = Moderator
            }); await _db.SaveICJCountriesAsync(SelectedCountries);

            MainWindow.Instance.NavigateRightFrame(new SetupPage());
        }

        [RelayCommand]
        private void Cancel()
        {
            MainWindow.Instance.NavigateRightFrame(new SetupPage());
        }
    }
}