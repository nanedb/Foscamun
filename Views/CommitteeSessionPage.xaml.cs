using Foscamun.Data;
using Foscamun.Models;
using Foscamun.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Foscamun.Views
{
    public partial class CommitteeSessionPage : Page
    {
        private CommitteeSessionViewModel _viewModel = null!;
        private readonly Committee _committee;
        private readonly SqliteDataAccess _dataAccess;

        public CommitteeSessionPage(Committee committee, string topic, int session, List<Country> presentCountries, SqliteDataAccess dataAccess)
        {
            InitializeComponent();

            _committee = committee;
            _dataAccess = dataAccess;
            _viewModel = new CommitteeSessionViewModel(committee, topic, session, presentCountries, NavigateToVoting, dataAccess);
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
            if (NavigationService != null)
            {
                // Torna alla RollCallPage
                var rollCallPage = new CommitteeRollCallPage(_committee, _dataAccess);
                NavigationService.Navigate(rollCallPage);
            }
        }
    }
}
