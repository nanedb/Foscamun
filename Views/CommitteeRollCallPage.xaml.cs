using Foscamun.Data;
using Foscamun.Models;
using Foscamun.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Foscamun.Views
{
    public partial class CommitteeRollCallPage : Page
    {
        private readonly CommitteeRollCallViewModel _viewModel;
        private readonly SqliteDataAccess _db;
        private readonly Committee _committee;

        public CommitteeRollCallPage(Committee committee, SqliteDataAccess db)
        {
            InitializeComponent();

            _db = db;
            _committee = committee;
            _viewModel = new CommitteeRollCallViewModel(committee, db, NavigateToSession);
            DataContext = _viewModel;

            // Mostra il messaggio dopo che la pagina è caricata
            Loaded += CommitteeRollCallPage_Loaded;
        }

        private async void CommitteeRollCallPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Rimuovi l'evento per evitare che venga chiamato più volte
            Loaded -= CommitteeRollCallPage_Loaded;

            // Aspetta un breve momento per permettere il rendering della pagina
            await System.Threading.Tasks.Task.Delay(100);

            // Verifica e mostra il messaggio se necessario
            _ = SqliteDataAccess.GetCommitteeLogoPath(_committee.Name, showWarning: true);
        }

        private void AvailableCountries_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_viewModel.SelectedCountry != null)
            {
                _viewModel.AvailableCountriesClicked(_viewModel.SelectedCountry);
            }
        }

        private void PresentCountries_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_viewModel.SelectedCountry != null)
            {
                _viewModel.PresentCountriesClicked(_viewModel.SelectedCountry);
            }
        }

        private void NavigateToSession(Committee committee, string topic, int session, List<Country> presentCountries)
        {
            var sessionPage = new CommitteeSessionPage(committee, topic, session, presentCountries, _db);
            NavigationService?.Navigate(sessionPage);
        }
    }
}
