using Foscamun2026.Data;
using Foscamun2026.Models;
using Foscamun2026.ViewModels;
using System.Windows.Controls;

namespace Foscamun2026.Views
{
    public partial class ICJRollCallPage : Page
    {
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
