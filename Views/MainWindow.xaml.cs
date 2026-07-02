using Foscamun.Data;
using Foscamun.Properties;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Foscamun.Views
{
    /// <summary>
    /// Application shell: collapsible sidebar on the left, animated
    /// navigation Frame (<see cref="RightFrame"/>) on the right.
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        // The sidebar starts collapsed; the hamburger button expands it.
        private bool _isPaneCollapsed = true;
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

        /// <summary>Roll call is only available once a committee has been selected.</summary>
        public bool IsRollCallEnabled => !string.IsNullOrWhiteSpace(Settings.Default.SelCommName);

        public SqliteDataAccess Db { get; }

        public static MainWindow Instance { get; private set; } = null!;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;
            Instance = this;
            Db = new SqliteDataAccess();

            RightFrame.Navigate(new HomePage());

            // Re-evaluate IsRollCallEnabled when the selected committee changes.
            Settings.Default.PropertyChanged += Settings_PropertyChanged;
        }

        private void Settings_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Settings.Default.SelCommName))
                OnPropertyChanged(nameof(IsRollCallEnabled));
        }

        // ------------------------- SIDEBAR -------------------------

        private void HamburgerBtn_Click(object sender, RoutedEventArgs e)
        {
            LeftColumn.Width = new GridLength(IsPaneCollapsed ? 180 : 56);
            IsPaneCollapsed = !IsPaneCollapsed;
        }

        private void HomeBtn_Click(object sender, RoutedEventArgs e) =>
            NavigateRightFrame(new HomePage());

        private void ConfigureBtn_Click(object sender, RoutedEventArgs e) =>
            NavigateRightFrame(new SetupPage());

        private void RollCallBtn_Click(object sender, RoutedEventArgs e)
        {
            var name = Settings.Default.SelCommName;
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Please select a committee first.", "No Committee Selected",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // ICJ is a special committee with its own roll call page.
            if (name == "ICJ")
            {
                NavigateRightFrame(new ICJRollCallPage(Db));
                return;
            }

            var committee = Db.GetCommitteeByNameAsync(name).GetAwaiter().GetResult();
            if (committee == null)
            {
                MessageBox.Show($"Committee '{name}' not found.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            NavigateRightFrame(new CommitteeRollCallPage(committee, Db));
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            var message = (string)Application.Current.FindResource("ExitConfirmMessage");
            var title = (string)Application.Current.FindResource("ExitConfirmTitle");

            if (MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question)
                == MessageBoxResult.Yes)
                Application.Current.Shutdown();
        }

        // ------------------- ANIMATED NAVIGATION -------------------

        /// <summary>Slides the current page out, navigates, then slides the new page in.</summary>
        public void NavigateRightFrame(Page page)
        {
            var slideOut = (Storyboard)Resources["RightFrameSlideOut"];

            slideOut.Completed -= SlideOutCompleted;
            slideOut.Completed += SlideOutCompleted;
            slideOut.Begin();

            void SlideOutCompleted(object? s, EventArgs e)
            {
                slideOut.Completed -= SlideOutCompleted;
                RightFrame.Navigate(page);
                ((Storyboard)Resources["RightFrameSlideIn"]).Begin();
            }
        }

        // --------------------------- MISC ---------------------------

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        protected override void OnClosed(EventArgs e)
        {
            Settings.Default.PropertyChanged -= Settings_PropertyChanged;
            base.OnClosed(e);
        }
    }
}
