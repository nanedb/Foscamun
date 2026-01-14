using Foscamun2026.Data;
using Foscamun2026.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Foscamun2026.Views
{
    public partial class AddCommitteePage : Page
    {
        private AddCommitteeViewModel Vm => (AddCommitteeViewModel)DataContext;

        public AddCommitteePage()
        {
            InitializeComponent();

            var db = ((App)Application.Current).Services!.GetRequiredService<SqliteDataAccess>();
            DataContext = new AddCommitteeViewModel(db);
        }

        private void AllCountries_Click(object sender, MouseButtonEventArgs e)
        {
            if (Vm.SelectedCountry != null)
                Vm.AllCountriesClicked(Vm.SelectedCountry);
        }

        private void SelectedCountries_Click(object sender, MouseButtonEventArgs e)
        {
            if (Vm.SelectedCountry != null)
                Vm.SelectedCountriesClicked(Vm.SelectedCountry);
        }
    }
}