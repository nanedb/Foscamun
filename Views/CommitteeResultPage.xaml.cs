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
        private readonly List<int> _inFavorIndices;
        private readonly List<int> _abstainedIndices;
        private readonly List<int> _againstIndices;
        private readonly CommitteeSessionPage? _sessionPage;

        public CommitteeResultPage(List<Country> voters, List<int> inFavorIndices, List<int> abstainedIndices, List<int> againstIndices, CommitteeSessionPage? sessionPage = null)
        {
            InitializeComponent();

            _voters = voters;
            _inFavorIndices = inFavorIndices;
            _abstainedIndices = abstainedIndices;
            _againstIndices = againstIndices;
            _sessionPage = sessionPage;
            Previous = new FlowLink("SessionLbl2", new RelayCommand(BackToSession));
            Next = new FlowLink("NextRoundLbl", new RelayCommand(StartNextRound));

            UpdateRoundText();
            DisplayResults();
        }

        private void UpdateRoundText()
        {
            RoundTextBlock.Text = Properties.Settings.Default.Lang switch
            {
                "fr" => $"Tour: {CommitteeVotingPage.Round}",
                "es" => $"Ronda: {CommitteeVotingPage.Round}",
                _ => $"Round: {CommitteeVotingPage.Round}"
            };
        }

        private void DisplayResults()
        {
            InFavorCount.Text = _inFavorIndices.Count.ToString();
            AbstainedCount.Text = _abstainedIndices.Count.ToString();
            AgainstCount.Text = _againstIndices.Count.ToString();

            foreach (var index in _inFavorIndices)
            {
                InFavorList.Items.Add(_voters[index]);
            }

            foreach (var index in _abstainedIndices)
            {
                AbstainedList.Items.Add(_voters[index]);
            }

            foreach (var index in _againstIndices)
            {
                AgainstList.Items.Add(_voters[index]);
            }
        }

        private void NewRoundBtn_Click(object sender, RoutedEventArgs e) => StartNextRound();

        private void StartNextRound()
        {
            CommitteeVotingPage.Round++;
            var committeeVotingPage = new CommitteeVotingPage(_voters, _sessionPage);
            NavigationService?.Navigate(committeeVotingPage);
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e) => BackToSession();

        private void BackToSession()
        {
            var result = MessageBox.Show(
                Properties.Settings.Default.Lang switch
                {
                    "fr" => "Êtes-vous sûr de vouloir revenir en arrière? Le vote en cours sera perdu.",
                    "es" => "¿Estás seguro de que quieres volver atrás? La votación en curso se perderá.",
                    _ => "Are you sure you want to go back? The current vote will be lost."
                },
                Properties.Settings.Default.Lang switch
                {
                    "fr" => "Confirmation",
                    "es" => "Confirmación",
                    _ => "Confirmation"
                },
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                if (_sessionPage != null && NavigationService != null)
                {
                    NavigationService.Navigate(_sessionPage);
                }
                else if (NavigationService != null && NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
            }
        }
    }
}
