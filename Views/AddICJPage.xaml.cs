using Foscamun2026.Data;
using Foscamun2026.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace Foscamun2026.Views
{
    public partial class AddICJPage : Page
    {
        private readonly AddICJViewModel _vm;

        public AddICJPage(SqliteDataAccess db)
        {
            InitializeComponent();
            _vm = new AddICJViewModel(db);
            DataContext = _vm;
        }

        private void AvailableCountries_Click(object sender, MouseButtonEventArgs e)
        {
            if (_vm.SelectedCountry != null)
                _vm.AddCountry(_vm.SelectedCountry);
        }

        private void SelectedCountries_Click(object sender, MouseButtonEventArgs e)
        {
            if (_vm.SelectedCountry != null)
                _vm.RemoveCountry(_vm.SelectedCountry);
        }
    }
}