using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Threading.Tasks;

namespace Foscamun2026.Views
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private bool _isPaneCollapsed = false;
        public bool IsPaneCollapsed
        {
            get => _isPaneCollapsed;
            set
            {
                if (_isPaneCollapsed == value) return;
                _isPaneCollapsed = value;
                OnPropertyChanged(nameof(IsPaneCollapsed));
            }
        }

        private HomePage _homeInstance;
        private SetupPage _setupInstance;


        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            _homeInstance = new HomePage();
            _setupInstance = new SetupPage();
            RightFrame.Navigate(_homeInstance);
        }

        // Handler chiamato dal Click dell'hamburger (definito in XAML)
        private void HamburgerBtn_Click(object sender, RoutedEventArgs e)
        {
            TogglePane();
        }

        private void BeginRightFrameSlideIn()
        {
            if (TryFindResource("RightFrameSlideIn") is Storyboard sb)
                sb.Begin(this);
        }

        // helper per avviare storyboard
       private void BeginStoryboardByKey(string key)
        {
            if (TryFindResource(key) is System.Windows.Media.Animation.Storyboard sb)
                sb.Begin(this, true);
        }

        private void TogglePane()
        {
            const double expandedWidth = 180;
            const double collapsedWidth = 56;

            if (IsPaneCollapsed)
            {
                LeftColumn.Width = new GridLength(expandedWidth);
                IsPaneCollapsed = false;
            }
            else
            {
                LeftColumn.Width = new GridLength(collapsedWidth);
                IsPaneCollapsed = true;
            }
        }

        private void HomeBtn_Click(object sender, RoutedEventArgs e)
        {
            // Torna alla Home (puoi riusare la stessa istanza se vuoi preservare stato)
            if (_homeInstance == null) _homeInstance = new HomePage();
            RightFrame.Navigate(_homeInstance);
        }

        private bool _isNavigating;

        private void ConfigureBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_isNavigating) return;
            _isNavigating = true;

            // Forza lo stato Normal sul pulsante che è stato cliccato
            if (sender is Control clickedCtrl)
                VisualStateManager.GoToState(clickedCtrl, "Normal", true);

            // crea l'istanza la prima volta e riusala
            if (_setupInstance == null)
            {
                _setupInstance = new SetupPage();

                // rimuovi prima per evitare doppie sottoscrizioni
                _setupInstance.RequestClose -= OnSetupRequestClose;
                _setupInstance.RequestClose += OnSetupRequestClose;
            }

            // Naviga effettivamente alla SetupPage
            RightFrame.Navigate(_setupInstance);

            // Avvia l'animazione di ingresso se la usi
            BeginRightFrameSlideIn();

            _isNavigating = false;
        }

        // handler separato per RequestClose
        private async void OnSetupRequestClose()
        {
            await PlaySlideOutAsync();
            NavigateToHome();
        }
        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            //var app = Application.Current as App;
            //if (app?.Services is null)
            //{
            //    MessageBox.Show("Errore: i servizi dell'applicazione non sono disponibili.", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            //    return;
            //}
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            // opzionale: conferma prima di uscire
            var result = MessageBox.Show("Sei sicuro di voler uscire?", "Conferma uscita", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        private Task PlaySlideOutAsync()
        {
            var tcs = new TaskCompletionSource<bool>();

            if (TryFindResource("RightFrameSlideOut") is System.Windows.Media.Animation.Storyboard sb)
            {
                void OnCompleted(object? sender, EventArgs e)
                {
                    sb.Completed -= OnCompleted;
                    tcs.SetResult(true);
                }

                sb.Completed += OnCompleted;
                sb.Begin(this, true);
            }
            else
            {
                tcs.SetResult(true);
            }

            return tcs.Task;
        }

        private void NavigateToHome()
        {
            if (_homeInstance == null) _homeInstance = new Foscamun2026.Views.HomePage();
            RightFrame.Navigate(_homeInstance);
            BeginStoryboardByKey("RightFrameSlideIn");
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    }
}
