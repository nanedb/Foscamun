using Foscamun2026.Data;
using Foscamun2026.Models;
using Foscamun2026.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Foscamun2026.Views
{
    public partial class SessionPage : Page
    {
        private SessionViewModel _viewModel = null!;

        public SessionPage(Committee committee, string topic, int session, List<Country> presentCountries, SqliteDataAccess dataAccess)
        {
            InitializeComponent();

            _viewModel = new SessionViewModel(committee, topic, session, presentCountries, NavigateToVoting, dataAccess);
            DataContext = _viewModel;
        }

        private void NavigateToVoting(List<Country> voters)
        {
            CastVotePage.Round = 1;
            var castVotePage = new CastVotePage(voters, this);
            NavigationService?.Navigate(castVotePage);
        }

        private void AvailableSpeakers_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_viewModel.SelectedSpeaker != null)
            {
                _viewModel.AddSpeakerCommand.Execute(_viewModel.SelectedSpeaker);
                // Reset della selezione
                _viewModel.SelectedSpeaker = null;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null && NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
    }
}
