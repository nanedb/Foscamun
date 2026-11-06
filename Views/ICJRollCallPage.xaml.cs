using Foscamun.Data;
using Foscamun.Models;
using Foscamun.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Foscamun.Views
{
    public partial class ICJRollCallPage : Page, IFlowPage
    {
        // Flow: back to Home, forward to the session once someone is present
        public string TitleKey => "RollCallBtn";
        public FlowLink? Previous => new(FlowLink.HomeKey, MainWindow.Instance.GoHomeCommand);
        public FlowLink? Next => new("SessionLbl2", _viewModel.ProceedCommand);

        private readonly ICJRollCallViewModel _viewModel;
        private readonly SqliteDataAccess _db;

        public ICJRollCallPage(SqliteDataAccess db)
        {
            InitializeComponent();

            _db = db;
            _viewModel = new ICJRollCallViewModel(db, NavigateToICJSession);
            DataContext = _viewModel;

        }

        private void AvailableMembers_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_viewModel.SelectedMember != null)
            {
                _viewModel.AvailableMembersClicked(_viewModel.SelectedMember);
            }
        }

        private void PresentMembers_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_viewModel.SelectedMember != null)
            {
                _viewModel.PresentMembersClicked(_viewModel.SelectedMember);
            }
        }

        private void NavigateToICJSession(string judge, string viceJudge1, string viceJudge2, string topic, int session, List<ICJRollCallMember> presentMembers)
        {
            var sessionPage = new ICJSessionPage(judge, viceJudge1, viceJudge2, topic, session, presentMembers, _db);
            NavigationService?.Navigate(sessionPage);
        }
    }
}
