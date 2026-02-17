using Foscamun2026.Models;
using System.Windows;
using System.Windows.Controls;

namespace Foscamun2026.Views
{
    public partial class FinalResultPage : Page
    {
        private readonly List<Country> _voters;
        private readonly List<int> _inFavorIndices;
        private readonly List<int> _againstIndices;
        private readonly SessionPage? _sessionPage;

        public FinalResultPage(List<Country> voters, List<int> inFavorIndices, List<int> againstIndices, SessionPage? sessionPage = null)
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
                    // Fallback: torna indietro come prima
                    bool sessionPageFound = false;

                    while (NavigationService.CanGoBack && !sessionPageFound)
                    {
                        NavigationService.GoBack();

                        if (NavigationService.Content is SessionPage)
                        {
                            sessionPageFound = true;
                        }
                    }
                }
            }
        }
    }
}
