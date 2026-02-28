using Foscamun.Models;
using System.Windows;
using System.Windows.Controls;

namespace Foscamun.Views
{
    public partial class CommitteeFinalResultPage : Page
    {
        private readonly List<Country> _voters;
        private readonly List<int> _inFavorIndices;
        private readonly List<int> _againstIndices;
        private readonly CommitteeSessionPage? _sessionPage;

        public CommitteeFinalResultPage(List<Country> voters, List<int> inFavorIndices, List<int> againstIndices, CommitteeSessionPage? sessionPage = null)
        {
            InitializeComponent();

            _voters = voters;
            _inFavorIndices = inFavorIndices;
            _againstIndices = againstIndices;
            _sessionPage = sessionPage;

            DisplayResults();
        }

        private void DisplayResults()
        {
            InFavorCount.Text = _inFavorIndices.Count.ToString();
            AgainstCount.Text = _againstIndices.Count.ToString();

            foreach (var index in _inFavorIndices)
            {
                InFavorList.Items.Add(_voters[index]);
            }

            foreach (var index in _againstIndices)
            {
                AgainstList.Items.Add(_voters[index]);
            }

            string outcome;
            // Simple majority: in case of tie, against wins
            if (_inFavorIndices.Count > _againstIndices.Count)
            {
                outcome = Properties.Settings.Default.Lang switch
                {
                    "fr" => "La motion est ADOPTÉE",
                    "es" => "La moción es ADOPTADA",
                    _ => "The motion PASSES"
                };
            }
            else
            {
                // Against wins if count is greater OR equal (tie)
                outcome = Properties.Settings.Default.Lang switch
                {
                    "fr" => "La motion est REJETÉE",
                    "es" => "La moción es RECHAZADA",
                    _ => "The motion FAILS"
                };
            }

            OutcomeTextBlock.Text = outcome;
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_sessionPage != null && NavigationService != null)
            {
                NavigationService.Navigate(_sessionPage);
            }
            else if (NavigationService != null && NavigationService.CanGoBack)
            {
                // Fallback: torna indietro come prima
                bool sessionPageFound = false;

                while (NavigationService.CanGoBack && !sessionPageFound)
                {
                    NavigationService.GoBack();

                    if (NavigationService.Content is CommitteeSessionPage)
                    {
                        sessionPageFound = true;
                    }
                }
            }
        }
    }
}
