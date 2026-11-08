using CommunityToolkit.Mvvm.Input;
using Foscamun.Models;
using System.Windows;
using System.Windows.Controls;

namespace Foscamun.Views
{
    public partial class ICJResultPage : Page, IFlowPage
    {
        // Flow: back to the session, forward to the next round
        public string TitleKey => "ResultLbl";
        public FlowLink? Previous { get; }
        public FlowLink? Next { get; }

        private readonly List<ICJRollCallMember> _voters;
        private readonly ICJSessionPage? _sessionPage;

        public ICJResultPage(List<ICJRollCallMember> voters, List<int> inFavorIndices, List<int> abstainedIndices, List<int> againstIndices, ICJSessionPage? sessionPage = null)
        {
            InitializeComponent();

            _voters = voters;
            _sessionPage = sessionPage;
            Previous = new FlowLink("SessionLbl2", new RelayCommand(BackToSession));
            Next = new FlowLink("NextRoundLbl", new RelayCommand(StartNextRound));

            // Round 1 needs a majority of the votes cast; round 2 ends here only on a tie.
            // Abstaining is allowed only in round 1.
            int round = ICJVotingPage.Round;
            string headline = (string)FindResource(round == 1 ? "NoMajorityLbl" : "TieLbl");
            Panel.Show(round, headline, Voters(inFavorIndices), Voters(abstainedIndices), Voters(againstIndices),
                       canAbstain: round == 1);

            Panel.BackRequested += (_, _) => BackToSession();
            Panel.NextRoundRequested += (_, _) => StartNextRound();
        }

        // Jurors: no flag, and the name without "Juror" in front
        private List<ResultVoter> Voters(List<int> indices) =>
            indices.Select(i => new ResultVoter(_voters[i].Member.Name ?? "", _voters[i].FlagPath)).ToList();

        private void StartNextRound()
        {
            ICJVotingPage.Round++;
            NavigationService?.Navigate(new ICJVotingPage(_voters, _sessionPage));
        }

        private void BackToSession()
        {
            if (AppDialog.Show((string)FindResource("LeaveVotingMsg"), (string)FindResource("ConfirmTitle"),
                               MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            ICJVotingPage.Round = 1;

            if (_sessionPage != null)
                NavigationService?.Navigate(_sessionPage);
            else if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }
    }
}
