using CommunityToolkit.Mvvm.Input;
using Foscamun.Models;
using System.Windows;
using System.Windows.Controls;

namespace Foscamun.Views
{
    public partial class ICJVotingPage : Page, IFlowPage
    {
        // Flow: back to the session; the result comes by itself after the last vote
        public string TitleKey => "VotingBtn";
        public FlowLink? Previous { get; }
        public FlowLink? Next => new("ResultLbl", null);

        private readonly List<ICJRollCallMember> _voters;
        private int _currentVoterIndex;
        private readonly int _numVoters;
        public static int Round { get; set; } = 1;

        /// <summary>Tallies of the rounds of the current vote, for the final result.</summary>
        public static List<RoundTally> History { get; } = new();
        private readonly ICJSessionPage? _sessionPage;

        private readonly List<int> _inFavorIndices = new();
        private readonly List<int> _abstainedIndices = new();
        private readonly List<int> _againstIndices = new();

        public ICJVotingPage(List<ICJRollCallMember> voters, ICJSessionPage? sessionPage = null)
        {
            InitializeComponent();

            // Sort voters by last name
            _voters = voters.OrderBy(v => GetLastName(v.Member.Name)).ToList();
            _numVoters = _voters.Count;
            _currentVoterIndex = 0;
            _sessionPage = sessionPage;
            Previous = new FlowLink("SessionLbl2", new RelayCommand(BackToSession));

            // No abstaining from the second round on
            Panel.CanAbstain = Round == 1;
            if (_sessionPage?.DataContext is ViewModels.ICJSessionViewModel session)
            {
                Panel.SetContext($"{FindResource("ICJTitle")} · {FindResource("SessionLbl2")} {session.Session}");
            }
            Panel.Approved += (_, _) => CastVote(_inFavorIndices);
            Panel.Abstained += (_, _) => CastVote(_abstainedIndices);
            Panel.Rejected += (_, _) => CastVote(_againstIndices);

            ShowVoter(_currentVoterIndex);
        }

        // Jurors: no flag, and the name without "Juror" in front
        private void ShowVoter(int index)
        {
            var voter = _voters[index];
            Panel.ShowVoter(Round, index, _numVoters, voter.FlagPath, voter.Member.Name ?? "",
                            _voters.Skip(index + 1).Select(v => v.Member.Name ?? ""));
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
            History.Add(new RoundTally(inFavorCount, _abstainedIndices.Count, againstCount, Round == 1,
                                       Round == 1 ? "NoMajorityLbl" : "TieLbl"));
            int abstainedCount = _abstainedIndices.Count;
            int totalVoters = _numVoters;

            if (Round == 1)
            {
                // Round 1: Gli Abstained non contano, la maggioranza si calcola sui voti validi
                int validVotes = inFavorCount + againstCount; // Esclusi gli astensioni
                int requiredVotes = (validVotes / 2) + 1;

                if (inFavorCount >= requiredVotes || againstCount >= requiredVotes)
                {
                    // Maggioranza raggiunta: vai a ICJFinalResultPage
                    var resultPage = new ICJFinalResultPage(_voters, _inFavorIndices, _abstainedIndices, _againstIndices, _sessionPage);
                    NavigationService?.Navigate(resultPage);
                }
                else
                {
                    // Nessuna maggioranza o parità: mostra risultati intermedi
                    var intermediatePage = new ICJResultPage(_voters, _inFavorIndices, _abstainedIndices, _againstIndices, _sessionPage);
                    NavigationService?.Navigate(intermediatePage);
                }
            }
            else if (Round == 2)
            {
                // Round 2: Controlla se c'è una maggioranza (no parità)
                if (inFavorCount > againstCount)
                {
                    // Maggioranza in favore: vai a ICJFinalResultPage
                    var resultPage = new ICJFinalResultPage(_voters, _inFavorIndices, _abstainedIndices, _againstIndices, _sessionPage);
                    NavigationService?.Navigate(resultPage);
                }
                else if (againstCount > inFavorCount)
                {
                    // Maggioranza contro: vai a ICJFinalResultPage
                    var resultPage = new ICJFinalResultPage(_voters, _inFavorIndices, _abstainedIndices, _againstIndices, _sessionPage);
                    NavigationService?.Navigate(resultPage);
                }
                else
                {
                    // Parità: mostra risultati intermedi prima del Round 3
                    var intermediatePage = new ICJResultPage(_voters, _inFavorIndices, _abstainedIndices, _againstIndices, _sessionPage);
                    NavigationService?.Navigate(intermediatePage);
                }
            }
            else // Round == 3
            {
                // Round 3: Se c'è parità, vincono gli Against
                bool tieInRound3 = inFavorCount == againstCount;
                var resultPage = new ICJFinalResultPage(_voters, _inFavorIndices, _abstainedIndices, _againstIndices, _sessionPage, tieInRound3);
                NavigationService?.Navigate(resultPage);
            }
        }

        /// <summary>
        /// Extracts the last name from a full name.
        /// Assumes format "FirstName LastName" or "FirstName MiddleName LastName".
        /// </summary>
        private static string GetLastName(string? fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return "";

            var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 0 ? parts[^1] : "";
        }
    }
}

