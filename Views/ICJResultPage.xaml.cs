using Foscamun2026.Models;
using System.Windows;
using System.Windows.Controls;

namespace Foscamun2026.Views
{
    public partial class ICJResultPage : Page
    {
        private readonly List<ICJRollCallMember> _voters;
        private readonly List<int> _inFavorIndices;
        private readonly List<int> _againstIndices;
        private readonly ICJSessionPage? _sessionPage;
        private readonly bool _tieInRound3;

        public ICJResultPage(List<ICJRollCallMember> voters, List<int> inFavorIndices, List<int> againstIndices, ICJSessionPage? sessionPage = null, bool tieInRound3 = false)
        {
            InitializeComponent();

            _voters = voters;
            _inFavorIndices = inFavorIndices;
            _againstIndices = againstIndices;
            _sessionPage = sessionPage;
            _tieInRound3 = tieInRound3;

            DisplayResults();
        }

        private void DisplayResults()
        {
            InFavorCount.Text = _inFavorIndices.Count.ToString();
            AgainstCount.Text = _againstIndices.Count.ToString();

            foreach (var index in _inFavorIndices)
            {
                InFavorList.Items.Add(_voters[index].DisplayName);
            }

            foreach (var index in _againstIndices)
            {
                AgainstList.Items.Add(_voters[index].DisplayName);
            }

            string outcome;
            
            // Se c'è parità nel terzo round, vincono gli Against
            if (_tieInRound3)
            {
                outcome = Properties.Settings.Default.Lang switch
                {
                    "fr" => "La motion est REJETÉE (Égalité au 3ème tour)",
                    "es" => "La moción es RECHAZADA (Empate en la 3ª ronda)",
                    _ => "The motion FAILS (Tie in Round 3)"
                };
            }
            else if (_inFavorIndices.Count > _againstIndices.Count)
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
                bool sessionPageFound = false;

                while (NavigationService.CanGoBack && !sessionPageFound)
                {
                    NavigationService.GoBack();

                    if (NavigationService.Content is ICJSessionPage)
                    {
                        sessionPageFound = true;
                    }
                }
            }
        }
    }
}
