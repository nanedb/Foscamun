using Foscamun2026.Data;
using Foscamun2026.ViewModels;
using System.Windows.Controls;

namespace Foscamun2026.Views
{
    public partial class ICJRollCallPage : Page
    {
        private readonly ICJRollCallViewModel _viewModel;

        public ICJRollCallPage(SqliteDataAccess db)
        {
            InitializeComponent();
            _viewModel = new ICJRollCallViewModel(db);
            DataContext = _viewModel;
        }

        private void AvailableMembers_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_viewModel.SelectedMember != null)
            {
                _viewModel.AvailableMembersClicked(_viewModel.SelectedMember);
            }
        }

        private void PresentMembers_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_viewModel.SelectedMember != null)
            {
                _viewModel.PresentMembersClicked(_viewModel.SelectedMember);
            }
        }
    }
}
