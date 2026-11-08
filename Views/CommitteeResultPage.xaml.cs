using CommunityToolkit.Mvvm.Input;
using Foscamun.Models;
using System.Windows;
using System.Windows.Controls;

namespace Foscamun.Views
{
    public partial class CommitteeResultPage : Page, IFlowPage
    {
        // Flow: back to the session, forward to the next round
        public string TitleKey => "ResultLbl";
        public FlowLink? Previous { get; }
        public FlowLink? Next { get; }

        private readonly List<Country> _voters;
        private readonly CommitteeSessionPage? _sessionPage;

        public CommitteeResultPage(List<Country> voters, List<int> inFavorIndices, List<int> abstainedIndices, List<int> againstIndices, CommitteeSessionPage? sessionPage = null)
        {
            InitializeComponent();

            _voters = voters;
            _sessionPage = sessionPage;
            Previous = new FlowLink("SessionLbl2", new RelayCommand(BackToSession));
            Next = new FlowLink("NextRoundLbl", new RelayCommand(StartNextRound));

            // Round 1 needs unanimity, round 2 a two-thirds majority (round 3 always ends the vote)
            int round = CommitteeVotingPage.Round;
            string headline = (string)FindResource(round == 1 ? "NoUnanimityLbl" : "NoTwoThirdsLbl");
            Panel.Show(round, headline, Voters(inFavorIndices), Voters(abstainedIndices), Voters(againstIndices));

            Panel.BackRequested += (_, _) => BackToSession();
            Panel.NextRoundRequested += (_, _) => StartNextRound();
        }

        private List<ResultVoter> Voters(List<int> indices) =>
            indices.Select(i => new ResultVoter(_voters[i].Name, _voters[i].FlagPath)).ToList();

        private void StartNextRound()
        {
            CommitteeVotingPage.Round++;
            NavigationService?.Navigate(new CommitteeVotingPage(_voters, _sessionPage));
        }

        private void BackToSession()
        {
            if (AppDialog.Show((string)FindResource("LeaveVotingMsg"), (string)FindResource("ConfirmTitle"),
                               MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            if (_sessionPage != null)
                NavigationService?.Navigate(_sessionPage);
            else if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }
    }
}
