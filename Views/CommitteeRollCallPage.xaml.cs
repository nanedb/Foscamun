using Foscamun2026.Data;
using Foscamun2026.Models;
using Foscamun2026.ViewModels;
using System.Windows.Controls;

namespace Foscamun2026.Views
{
    public partial class CommitteeRollCallPage : Page
    {
        private readonly CommitteeRollCallViewModel _viewModel;
        private readonly SqliteDataAccess _db;

        public CommitteeRollCallPage(Committee committee, SqliteDataAccess db)
        {
            InitializeComponent();

            _db = db;
            _viewModel = new CommitteeRollCallViewModel(committee, db, NavigateToSession);
            DataContext = _viewModel;
        }

        private void AvailableCountries_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_viewModel.SelectedCountry != null)
            {
                _viewModel.AvailableCountriesClicked(_viewModel.SelectedCountry);
            }
        }

        private void PresentCountries_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_viewModel.SelectedCountry != null)
            {
                _viewModel.PresentCountriesClicked(_viewModel.SelectedCountry);
            }
        }

        private void NavigateToSession(Committee committee, string topic, int session, List<Country> presentCountries)
        {
            var sessionPage = new CommitteeSessionPage(committee, topic, session, presentCountries, _db);
            NavigationService?.Navigate(sessionPage);
        }
    }
}
