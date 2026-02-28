using Foscamun.Data;
using Foscamun.Models;
using Foscamun.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Foscamun.Views
{
    public partial class ICJRollCallPage : Page
    {
        private readonly ICJRollCallViewModel _viewModel;
        private readonly SqliteDataAccess _db;

        public ICJRollCallPage(SqliteDataAccess db)
        {
            InitializeComponent();

            _db = db;
            _viewModel = new ICJRollCallViewModel(db, NavigateToICJSession);
            DataContext = _viewModel;

            // Mostra il messaggio dopo che la pagina è caricata
            Loaded += ICJRollCallPage_Loaded;
        }

        private async void ICJRollCallPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Rimuovi l'evento per evitare che venga chiamato più volte
            Loaded -= ICJRollCallPage_Loaded;

            // Aspetta un breve momento per permettere il rendering della pagina
            await System.Threading.Tasks.Task.Delay(100);

            // Verifica e mostra il messaggio se necessario per ICJ
            _ = SqliteDataAccess.GetCommitteeLogoPath("ICJ", showWarning: true);
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

        private void NavigateToICJSession(string judge, string viceJudge1, string viceJudge2, string topic, int session, List<ICJRollCallMember> presentMembers)
        {
            var sessionPage = new ICJSessionPage(judge, viceJudge1, viceJudge2, topic, session, presentMembers, _db);
            NavigationService?.Navigate(sessionPage);
        }
    }
}
