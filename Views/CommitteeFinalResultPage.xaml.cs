using CommunityToolkit.Mvvm.Input;
using Foscamun.Models;
using System.Windows.Controls;

namespace Foscamun.Views
{
    public partial class CommitteeFinalResultPage : Page, IFlowPage
    {
        // Flow: back to the session, or home
        public string TitleKey => "FinalResultLbl";
        public FlowLink? Previous { get; }
        public FlowLink? Next => new(FlowLink.HomeKey, MainWindow.Instance.GoHomeCommand);

        private readonly List<Country> _voters;
        private readonly CommitteeSessionPage? _sessionPage;

        public CommitteeFinalResultPage(List<Country> voters, List<int> inFavorIndices, List<int> abstainedIndices,
                                        List<int> againstIndices, CommitteeSessionPage? sessionPage = null)
        {
            InitializeComponent();

            _voters = voters;
            _sessionPage = sessionPage;
            Previous = new FlowLink("SessionLbl2", new RelayCommand(BackToSession));

            // Simple majority: a tie goes against
            bool adopted = inFavorIndices.Count > againstIndices.Count;
            string? context = _sessionPage?.DataContext is ViewModels.CommitteeSessionViewModel session
                ? $"{session.CommitteeName} · {FindResource("SessionLbl2")} {session.Session}"
                : null;

            Panel.Show(context, (string)FindResource(adopted ? "ResolutionAdoptedLbl" : "ResolutionRejectedLbl"),
                       adopted, CommitteeVotingPage.History,
                       Voters(inFavorIndices), Voters(abstainedIndices), Voters(againstIndices));
            Panel.BackRequested += (_, _) => BackToSession();
        }

        private List<ResultVoter> Voters(List<int> indices) =>
            indices.Select(i => new ResultVoter(_voters[i].Name, _voters[i].FlagPath)).ToList();

        // The vote is over: nothing to lose, no question
        private void BackToSession()
        {
            if (_sessionPage != null)
            {
                NavigationService?.Navigate(_sessionPage);
                return;
            }

            // Without the session page, go back through the history until it shows up
            while (NavigationService?.CanGoBack == true)
            {
                NavigationService.GoBack();
                if (NavigationService.Content is CommitteeSessionPage)
                    break;
            }
        }
    }
}
