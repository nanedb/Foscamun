using CommunityToolkit.Mvvm.Input;
using Foscamun.Data;
using Foscamun.Models;
using Foscamun.ViewModels;
using System.Windows.Controls;

namespace Foscamun.Views
{
    public partial class CommitteeSessionPage : Page, IFlowPage
    {
        // Flow: back to a new roll call, forward to voting
        public string TitleKey => "SessionLbl2";
        public FlowLink? Previous { get; }
        public FlowLink? Next => new("VotingBtn", _viewModel.OpenVotingCommand);

        private readonly CommitteeSessionViewModel _viewModel;
        private readonly Committee _committee;
        private readonly SqliteDataAccess _dataAccess;

        public CommitteeSessionPage(Committee committee, string topic, int session, List<Country> presentCountries, SqliteDataAccess dataAccess)
        {
            InitializeComponent();

            _committee = committee;
            _dataAccess = dataAccess;
            _viewModel = new CommitteeSessionViewModel(committee, topic, session, presentCountries, NavigateToVoting, dataAccess);
            DataContext = _viewModel;

            Previous = new FlowLink("RollCallBtn", new RelayCommand(BackToRollCall));
        }

        private void NavigateToVoting(List<Country> voters)
        {
            CommitteeVotingPage.Round = 1;
            NavigationService?.Navigate(new CommitteeVotingPage(voters, this));
        }

        private void BackToRollCall()
        {
            NavigationService?.Navigate(new CommitteeRollCallPage(_committee, _dataAccess));
        }
    }
}
