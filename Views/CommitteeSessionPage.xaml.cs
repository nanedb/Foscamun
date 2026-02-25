using Foscamun2026.Data;
using Foscamun2026.Models;
using Foscamun2026.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Foscamun2026.Views
{
    public partial class CommitteeSessionPage : Page
    {
        private SessionViewModel _viewModel = null!;

        public CommitteeSessionPage(Committee committee, string topic, int session, List<Country> presentCountries, SqliteDataAccess dataAccess)
        {
            InitializeComponent();

            _viewModel = new SessionViewModel(committee, topic, session, presentCountries, NavigateToVoting, dataAccess);
            DataContext = _viewModel;
        }

        private void NavigateToVoting(List<Country> voters)
        {
            CommitteeVotingPage.Round = 1;
            var committeeVotingPage = new CommitteeVotingPage(voters, this);
            NavigationService?.Navigate(committeeVotingPage);
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
