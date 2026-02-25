using Foscamun2026.Models;
using System.Windows;
using System.Windows.Controls;

namespace Foscamun2026.Views
{
    public partial class ICJResultIntermediatePage : Page
    {
        private readonly List<ICJRollCallMember> _voters;
        private readonly List<int> _inFavorIndices;
        private readonly List<int> _abstainedIndices;
        private readonly List<int> _againstIndices;
        private readonly ICJSessionPage? _sessionPage;

        public ICJResultIntermediatePage(List<ICJRollCallMember> voters, List<int> inFavorIndices, List<int> abstainedIndices, List<int> againstIndices, ICJSessionPage? sessionPage = null)
        {
            InitializeComponent();

            _voters = voters;
            _inFavorIndices = inFavorIndices;
            _abstainedIndices = abstainedIndices;
            _againstIndices = againstIndices;
            _sessionPage = sessionPage;

            UpdateRoundText();
            DisplayResults();
        }

        private void UpdateRoundText()
        {
            RoundTextBlock.Text = Properties.Settings.Default.Lang switch
            {
                "fr" => $"Tour: {ICJVotingPage.Round}",
                "es" => $"Ronda: {ICJVotingPage.Round}",
                _ => $"Round: {ICJVotingPage.Round}"
            };
        }

        private void DisplayResults()
        {
            InFavorCount.Text = _inFavorIndices.Count.ToString();
            AgainstCount.Text = _againstIndices.Count.ToString();

            foreach (var index in _inFavorIndices)
            {
                InFavorList.Items.Add(_voters[index].Member.Name);
            }

            foreach (var index in _againstIndices)
            {
                AgainstList.Items.Add(_voters[index].Member.Name);
            }

            // Se siamo nel Round 2 o 3, nascondi la colonna Abstained
            if (ICJVotingPage.Round >= 2)
            {
                AbstainedGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                AbstainedCount.Text = _abstainedIndices.Count.ToString();
                foreach (var index in _abstainedIndices)
                {
                    AbstainedList.Items.Add(_voters[index].Member.Name);
                }
            }
        }

        private void NewRoundBtn_Click(object sender, RoutedEventArgs e)
        {
            ICJVotingPage.Round++;
            var votingPage = new ICJVotingPage(_voters, _sessionPage);
            NavigationService?.Navigate(votingPage);
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
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
                // Reset Round to 1 when going back
                ICJVotingPage.Round = 1;
                
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
