using Foscamun.Data;
using Foscamun.Properties;
using Foscamun.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace Foscamun.Views
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

        // ? NUOVA PROPRIETÀ
        public bool IsRollCallEnabled => !string.IsNullOrWhiteSpace(Settings.Default.SelCommName);

        public SqliteDataAccess Db { get; }

        public static MainWindow Instance { get; private set; } = null!;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            Instance = this;

            Db = new SqliteDataAccess();

            // Prima pagina
            RightFrame.Navigate(new HomePage());

            // ? Monitora i cambiamenti delle impostazioni
            Settings.Default.PropertyChanged += Settings_PropertyChanged;
        }

        // ? NUOVO METODO
        private void Settings_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Settings.Default.SelCommName))
            {
                OnPropertyChanged(nameof(IsRollCallEnabled));
            }
        }

        private void RightFrame_Navigated(object sender, NavigationEventArgs e)
        {
            var slideIn = (Storyboard)Resources["RightFrameSlideIn"];
            slideIn.Begin();
        }

        // -------------------------
        //  HAMBURGER
        // -------------------------
        private void HamburgerBtn_Click(object sender, RoutedEventArgs e)
        {
            TogglePane();
        }

        private void TogglePane()
        {
            const double expandedWidth = 180;
            const double collapsedWidth = 56;

            LeftColumn.Width = new GridLength(IsPaneCollapsed ? expandedWidth : collapsedWidth);
            IsPaneCollapsed = !IsPaneCollapsed;
        }

        // -------------------------
        //  NAVIGAZIONE MENU
        // -------------------------
        private void HomeBtn_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(((Button)sender).IsPressed);
            NavigateRightFrame(new HomePage());
        }

        private bool _isNavigating;

        private void ConfigureBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_isNavigating) return;
            _isNavigating = true;

            NavigateRightFrame(new SetupPage());

            _isNavigating = false;
        }

        // ? NUOVO METODO per Roll Call
        private void RollCallBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedCommitteeName = Settings.Default.SelCommName;

            if (string.IsNullOrWhiteSpace(selectedCommitteeName))
            {
                MessageBox.Show("Please select a committee first.", "No Committee Selected", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // ICJ -> ICJRollCallPage
            if (selectedCommitteeName == "ICJ")
            {
                var icjPage = new ICJRollCallPage(Db);
                NavigateRightFrame(icjPage);
                return;
            }

            // Committee normale -> Carica il Committee dal DB
            var committee = Db.GetCommitteeByNameAsync(selectedCommitteeName).GetAwaiter().GetResult();

            if (committee == null)
            {
                MessageBox.Show($"Committee '{selectedCommitteeName}' not found.", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var committeePage = new CommitteeRollCallPage(committee, Db);
            NavigateRightFrame(committeePage);
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            var message = (string)Application.Current.FindResource("ExitConfirmMessage");
            var title = (string)Application.Current.FindResource("ExitConfirmTitle");

            var result = MessageBox.Show(message, title,
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
                Application.Current.Shutdown();
        }

        // -------------------------
        //  NAVIGAZIONE ANIMATA
        // -------------------------
        public void NavigateRightFrame(Page page)
        {
            var slideOut = (Storyboard)Resources["RightFrameSlideOut"];

            // Rimuovi eventuali handler precedenti
            slideOut.Completed -= SlideOutCompleted;

            // Aggiungi handler
            slideOut.Completed += SlideOutCompleted;

            void SlideOutCompleted(object? s, EventArgs e)
            {
                slideOut.Completed -= SlideOutCompleted; // evita duplicazioni

                // Imposta la nuova pagina nel ContentControl
                RightFrame.Navigate(page);

                // Avvia lo slide-in corretto
                var slideIn = (Storyboard)Resources["RightFrameSlideIn"];
                slideIn.Begin();
            }

            slideOut.Begin();
        }

        // -------------------------
        //  INotifyPropertyChanged
        // -------------------------
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // ? CLEANUP
        protected override void OnClosed(EventArgs e)
        {
            Settings.Default.PropertyChanged -= Settings_PropertyChanged;
            base.OnClosed(e);
        }
    }
}
