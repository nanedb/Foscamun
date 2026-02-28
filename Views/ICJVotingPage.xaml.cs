using Foscamun.Models;
using System.Windows;
using System.Windows.Controls;

namespace Foscamun.Views
{
    public partial class ICJVotingPage : Page
    {
        private readonly List<ICJRollCallMember> _voters;
        private int _currentVoterIndex;
        private readonly int _numVoters;
        public static int Round { get; set; } = 1;
        private readonly ICJSessionPage? _sessionPage;

        private readonly List<int> _inFavorIndices = new();
        private readonly List<int> _abstainedIndices = new();
        private readonly List<int> _againstIndices = new();

        public ICJVotingPage(List<ICJRollCallMember> voters, ICJSessionPage? sessionPage = null)
        {
            InitializeComponent();

            _voters = new List<ICJRollCallMember>(voters);
            _numVoters = _voters.Count;
            _currentVoterIndex = 0;
            _sessionPage = sessionPage;

            UpdateRoundText();

            // Nelle round 2 e 3, nascondi il pulsante Abstain
            if (Round >= 2)
            {
                AbstainBtn.Visibility = Visibility.Collapsed;
            }

            ShowVoter(_currentVoterIndex);
        }

        private void UpdateRoundText()
        {
            RoundTextBlock.Text = Properties.Settings.Default.Lang switch
            {
                "fr" => $"Tour: {Round}",
                "es" => $"Ronda: {Round}",
                _ => $"Round: {Round}"
            };
        }

        private void ShowVoter(int index)
        {
            var voter = _voters[index];
            // Mostra solo il nome senza "Juror" davanti
            CurrentVoterText.Text = voter.Member.Name ?? "";
        }

        private void FavorBtn_Click(object sender, RoutedEventArgs e)
        {
            _inFavorIndices.Add(_currentVoterIndex);
            _currentVoterIndex++;
            ProcessVote();
        }

        private void AbstainBtn_Click(object sender, RoutedEventArgs e)
        {
            _abstainedIndices.Add(_currentVoterIndex);
            _currentVoterIndex++;
            ProcessVote();
        }

        private void AgainstBtn_Click(object sender, RoutedEventArgs e)
        {
            _againstIndices.Add(_currentVoterIndex);
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
                    var resultPage = new ICJFinalResultPage(_voters, _inFavorIndices, _againstIndices, _sessionPage);
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
                    var resultPage = new ICJFinalResultPage(_voters, _inFavorIndices, _againstIndices, _sessionPage);
                    NavigationService?.Navigate(resultPage);
                }
                else if (againstCount > inFavorCount)
                {
                    // Maggioranza contro: vai a ICJFinalResultPage
                    var resultPage = new ICJFinalResultPage(_voters, _inFavorIndices, _againstIndices, _sessionPage);
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
                var resultPage = new ICJFinalResultPage(_voters, _inFavorIndices, _againstIndices, _sessionPage, tieInRound3);
                NavigationService?.Navigate(resultPage);
            }
        }
    }
}
