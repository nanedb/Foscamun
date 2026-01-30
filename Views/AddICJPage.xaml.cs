using Foscamun2026.Data;
using Foscamun2026.Models;
using Foscamun2026.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace Foscamun2026.Views
{
    public partial class AddICJPage : Page
    {
        private readonly SqliteDataAccess _db;

        public AddICJPage(SqliteDataAccess db)
        {
            InitializeComponent();
            _db = db;
            DataContext = new AddICJViewModel(db);
        }

        private void AvailableCountries_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is AddICJViewModel vm && vm.SelectedCountry is Country c)
                vm.AddCountry(c);
        }

        private void SelectedCountries_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is AddICJViewModel vm && vm.SelectedCountry is Country c)
                vm.RemoveCountry(c);
        }
    }
}