using CommunityToolkit.Mvvm.Input;
using Foscamun.Models;
using System.Windows;
using System.Windows.Controls;

namespace Foscamun.Views
{
    public partial class CommitteeVotingPage : Page, IFlowPage
    {
        // Flow: back to the session; the result comes by itself after the last vote
        public string TitleKey => "VotingBtn";
        public FlowLink? Previous { get; }
        public FlowLink? Next => new("ResultLbl", null);

        private readonly List<Country> _voters;
        private int _currentVoterIndex;
        private readonly int _numVoters;
        public static int Round { get; set; } = 1;

        /// <summary>Tallies of the rounds of the current vote, for the final result.</summary>
        public static List<RoundTally> History { get; } = new();
        private readonly CommitteeSessionPage? _sessionPage;

        private readonly List<int> _inFavorIndices = new();
        private readonly List<int> _abstainedIndices = new();
        private readonly List<int> _againstIndices = new();

        public CommitteeVotingPage(List<Country> voters, CommitteeSessionPage? sessionPage = null)
        {
            InitializeComponent();

            _voters = new List<Country>(voters);
            _numVoters = _voters.Count;
            _currentVoterIndex = 0;
            _sessionPage = sessionPage;
            Previous = new FlowLink("SessionLbl2", new RelayCommand(BackToSession));

            // No abstaining in the third round
            Panel.CanAbstain = Round != 3;
            if (_sessionPage?.DataContext is ViewModels.CommitteeSessionViewModel session)
            {
                Panel.SetContext($"{session.CommitteeName} · {FindResource("SessionLbl2")} {session.Session}");
            }
            Panel.Approved += (_, _) => CastVote(_inFavorIndices);
            Panel.Abstained += (_, _) => CastVote(_abstainedIndices);
            Panel.Rejected += (_, _) => CastVote(_againstIndices);

            ShowVoter(_currentVoterIndex);
        }

        private void ShowVoter(int index)
        {
            var voter = _voters[index];
            Panel.ShowVoter(Round, index, _numVoters, voter.FlagPath, voter.Name,
                            _voters.Skip(index + 1).Select(v => v.Name));
        }

        /// <summary>
        /// Goes back to the session; votes already cast would be lost, so it asks first.
        /// </summary>
        private void BackToSession()
        {
            if (_currentVoterIndex > 0 &&
                AppDialog.Show((string)FindResource("LeaveVotingMsg"), (string)FindResource("ConfirmTitle"),
                                MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            if (_sessionPage != null)
                NavigationService?.Navigate(_sessionPage);
            else if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }

        /// <summary>Records the current voter's vote and moves on.</summary>
        private void CastVote(List<int> votes)
        {
            // Votes arriving after the last voter (a quick key press) are ignored
            if (_currentVoterIndex >= _numVoters) return;

            votes.Add(_currentVoterIndex);
            _currentVoterIndex++;
            ProcessVote();
        }

        private void ProcessVote()
        {
            if (_currentVoterIndex < _numVoters)
            {
                ShowVoter(_currentVoterIndex);
            }
            else
            {
                ShowResults();
            }
        }

        private void ShowResults()
        {
            int inFavorCount = _inFavorIndices.Count;
            int againstCount = _againstIndices.Count;

            // A new vote starts its history again
            if (Round == 1)
                History.Clear();
            History.Add(new RoundTally(inFavorCount, _abstainedIndices.Count, againstCount, Round != 3,
                                       Round == 1 ? "NoUnanimityLbl" : "NoTwoThirdsLbl"));
            int totalVoters = _numVoters;

            if (Round == 1)
            {
                // Round 1: Check for unanimity (all in favor OR all against)
                if (inFavorCount == totalVoters || againstCount == totalVoters)
                {
                    // Unanimity: go directly to CommitteeFinalResultPage
                    var finalResultPage = new CommitteeFinalResultPage(_voters, _inFavorIndices, _abstainedIndices, _againstIndices, _sessionPage);
                    NavigationService?.Navigate(finalResultPage);
                }
                else
                {
                    // No unanimity: show results and allow round 2
                    var resultPage = new CommitteeResultPage(_voters, _inFavorIndices, _abstainedIndices, _againstIndices, _sessionPage);
                    NavigationService?.Navigate(resultPage);
                }
            }
            else if (Round == 2)
            {
                // Round 2: Check for 2/3 majority on votes cast (excluding abstentions)
                int votesCast = inFavorCount + againstCount;
                double twoThirds = (votesCast * 2.0) / 3.0;

                if (inFavorCount >= twoThirds || againstCount >= twoThirds)
                {
                    // 2/3 majority achieved: go to CommitteeFinalResultPage
                    var finalResultPage = new CommitteeFinalResultPage(_voters, _inFavorIndices, _abstainedIndices, _againstIndices, _sessionPage);
                    NavigationService?.Navigate(finalResultPage);
                }
                else
                {
                    // No 2/3 majority: show results and allow round 3
                    var resultPage = new CommitteeResultPage(_voters, _inFavorIndices, _abstainedIndices, _againstIndices, _sessionPage);
                    NavigationService?.Navigate(resultPage);
                }
            }
            else // Round == 3
            {
                // Round 3: Always go to CommitteeFinalResultPage (simple majority, ties go against)
                var finalResultPage = new CommitteeFinalResultPage(_voters, _inFavorIndices, _abstainedIndices, _againstIndices, _sessionPage);
                NavigationService?.Navigate(finalResultPage);
            }
        }
    }
}
