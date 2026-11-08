using CommunityToolkit.Mvvm.Input;
using Foscamun.Models;
using System.Windows.Controls;

namespace Foscamun.Views
{
    public partial class ICJFinalResultPage : Page, IFlowPage
    {
        // Flow: back to the session, or home
        public string TitleKey => "FinalResultLbl";
        public FlowLink? Previous { get; }
        public FlowLink? Next => new(FlowLink.HomeKey, MainWindow.Instance.GoHomeCommand);

        private readonly List<ICJRollCallMember> _voters;
        private readonly ICJSessionPage? _sessionPage;

        /// <param name="tieInRound3">A tie in the last round: the motion is rejected.</param>
        public ICJFinalResultPage(List<ICJRollCallMember> voters, List<int> inFavorIndices, List<int> abstainedIndices,
                                  List<int> againstIndices, ICJSessionPage? sessionPage = null, bool tieInRound3 = false)
        {
            InitializeComponent();

            _voters = voters;
            _sessionPage = sessionPage;
            Previous = new FlowLink("SessionLbl2", new RelayCommand(BackToSession));

            bool adopted = !tieInRound3 && inFavorIndices.Count > againstIndices.Count;
            string? context = _sessionPage?.DataContext is ViewModels.ICJSessionViewModel session
                ? $"{FindResource("ICJTitle")} · {FindResource("SessionLbl2")} {session.Session}"
                : null;

            Panel.Show(context, (string)FindResource(adopted ? "MotionAdoptedLbl" : "MotionRejectedLbl"),
                       adopted, ICJVotingPage.History,
                       Voters(inFavorIndices), Voters(abstainedIndices), Voters(againstIndices));
            Panel.BackRequested += (_, _) => BackToSession();
        }

        // Jurors have no flag
        private List<ResultVoter> Voters(List<int> indices) =>
            indices.Select(i => new ResultVoter(_voters[i].Member.Name ?? "", null)).ToList();

        // The vote is over: nothing to lose, no question
        private void BackToSession()
        {
            ICJVotingPage.Round = 1;

            if (_sessionPage != null)
            {
                NavigationService?.Navigate(_sessionPage);
                return;
            }

            // Without the session page, go back through the history until it shows up
            while (NavigationService?.CanGoBack == true)
            {
                NavigationService.GoBack();
                if (NavigationService.Content is ICJSessionPage)
                    break;
            }
        }
    }
}
