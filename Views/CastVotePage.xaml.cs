using Foscamun2026.Models;
using System.Windows;
using System.Windows.Controls;

namespace Foscamun2026.Views
{
    public partial class CastVotePage : Page
    {
        private readonly List<Country> _voters;
        private int _currentVoterIndex;
        private readonly int _numVoters;
        public static int Round { get; set; } = 1;
        private readonly SessionPage? _sessionPage;

        private readonly List<int> _inFavorIndices = new();
        private readonly List<int> _abstainedIndices = new();
        private readonly List<int> _againstIndices = new();

        public CastVotePage(List<Country> voters, SessionPage? sessionPage = null)
        {
            InitializeComponent();

            _voters = new List<Country>(voters);
            _numVoters = _voters.Count;
            _currentVoterIndex = 0;
            _sessionPage = sessionPage;

            UpdateRoundText();

            if (Round == 3)
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
            CurrentVoterFlag.Source = new Uri(voter.FlagPath);
            CurrentVoterText.Text = voter.Name;
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
            if (Round == 3)
            {
                var finalResultPage = new FinalResultPage(_voters, _inFavorIndices, _againstIndices, _sessionPage);
                NavigationService?.Navigate(finalResultPage);
            }
            else
            {
                var resultPage = new ResultPage(_voters, _inFavorIndices, _abstainedIndices, _againstIndices, _sessionPage);
                NavigationService?.Navigate(resultPage);
            }
        }
    }
}
