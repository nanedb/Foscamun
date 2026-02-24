using Foscamun2026.Data;
using Foscamun2026.Models;
using Foscamun2026.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace Foscamun2026.Views
{
    public partial class ICJSessionPage : Page
    {
        private ICJSessionViewModel _viewModel = null!;

        public ICJSessionPage(string judge, string viceJudge1, string viceJudge2, string topic, int session, List<ICJRollCallMember> presentMembers, SqliteDataAccess dataAccess)
        {
            InitializeComponent();

            _viewModel = new ICJSessionViewModel(judge, viceJudge1, viceJudge2, topic, session, presentMembers, dataAccess);
            DataContext = _viewModel;
        }

        private void AvailableSpeakers_Click(object sender, MouseButtonEventArgs e)
        {
            if (_viewModel.SelectedSpeaker != null)
            {
                _viewModel.AddSpeakerCommand.Execute(_viewModel.SelectedSpeaker);
            }
        }

        private void BackButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
    }
}
