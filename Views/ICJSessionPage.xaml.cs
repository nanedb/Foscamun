using CommunityToolkit.Mvvm.Input;
using Foscamun.Data;
using Foscamun.Models;
using Foscamun.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace Foscamun.Views
{
    public partial class ICJSessionPage : Page, IFlowPage
    {
        // Flow: back to a new roll call, forward to voting
        public string TitleKey => "SessionLbl2";
        public FlowLink? Previous { get; }
        public FlowLink? Next => new("VotingBtn", _viewModel.OpenVotingCommand);

        private ICJSessionViewModel _viewModel = null!;
        private readonly SqliteDataAccess _dataAccess;

        public ICJSessionPage(string judge, string viceJudge1, string viceJudge2, string topic, int session, List<ICJRollCallMember> presentMembers, SqliteDataAccess dataAccess)
        {
            InitializeComponent();

            _dataAccess = dataAccess;
            _viewModel = new ICJSessionViewModel(judge, viceJudge1, viceJudge2, topic, session, presentMembers, NavigateToVoting, dataAccess);
            DataContext = _viewModel;

            Previous = new FlowLink("RollCallBtn", new RelayCommand(BackToRollCall));
        }

        private void NavigateToVoting(List<ICJRollCallMember> voters)
        {
            ICJVotingPage.Round = 1;
            var votingPage = new ICJVotingPage(voters, this);
            NavigationService?.Navigate(votingPage);
        }

        private void AvailableSpeakers_Click(object sender, MouseButtonEventArgs e)
        {
            if (_viewModel.SelectedSpeaker != null)
            {
                _viewModel.AddSpeakerCommand.Execute(_viewModel.SelectedSpeaker);
            }
        }

        private void BackButton_Click(object sender, System.Windows.RoutedEventArgs e) => BackToRollCall();

        private void BackToRollCall()
        {
            if (NavigationService != null)
            {
                // Torna alla ICJRollCallPage
                var rollCallPage = new ICJRollCallPage(_dataAccess);
                NavigationService.Navigate(rollCallPage);
            }
        }
    }
}
