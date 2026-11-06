using Foscamun.Data;
using Foscamun.Models;
using Foscamun.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Foscamun.Views
{
    public partial class CommitteeRollCallPage : Page, IFlowPage
    {
        // Flow: back to Home, forward to the session once someone is present
        public string TitleKey => "RollCallBtn";
        public FlowLink? Previous => new(FlowLink.HomeKey, MainWindow.Instance.GoHomeCommand);
        public FlowLink? Next => new("SessionLbl2", _viewModel.ProceedCommand);

        private readonly CommitteeRollCallViewModel _viewModel;
        private readonly SqliteDataAccess _db;
        private readonly Committee _committee;

        public CommitteeRollCallPage(Committee committee, SqliteDataAccess db)
        {
            InitializeComponent();

            _db = db;
            _committee = committee;
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
