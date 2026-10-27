using CommunityToolkit.Mvvm.Input;
using Foscamun.Data;
using Foscamun.Properties;
using SharpVectors.Converters;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

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

        /// <summary>Opens the roll call of the selected committee.</summary>
        public IRelayCommand OpenRollCallCommand { get; }

        /// <summary>Goes back to the Home page.</summary>
        public IRelayCommand GoHomeCommand { get; }

        // Last page of the flow shown, and the committee it belongs to: Setup and its
        // edit pages keep it in the sidebar, to go back to it with one click.
        private IFlowPage? _flowPage;
        private string _flowCommittee = "";

        public MainWindow()
        {
            OpenRollCallCommand = new RelayCommand(OpenRollCall, () => IsRollCallEnabled);
            GoHomeCommand = new RelayCommand(() => NavigateRightFrame(new HomePage()));

            InitializeComponent();

            // On short screens keep the whole window (title bar and bottom buttons) visible
            Height = Math.Min(Height, SystemParameters.WorkArea.Height);

            DataContext = this;
            Instance = this;
            Db = new SqliteDataAccess();

            RightFrame.Navigate(new HomePage());

            // Re-evaluate the roll call link when the selected committee changes.
            Settings.Default.PropertyChanged += Settings_PropertyChanged;
        }

        private void Settings_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(Settings.Default.SelCommName))
                return;

            OpenRollCallCommand.NotifyCanExecuteChanged();

            // The flow kept for Setup belongs to the previous committee: start again from Home
            if (Settings.Default.SelCommName != _flowCommittee && _flowPage is not HomePage)
            {
                _flowPage = new HomePage();
                _flowCommittee = Settings.Default.SelCommName;
                if (RightFrame.Content is not IFlowPage)
                    UpdateSidebar(RightFrame.Content);
            }
        }

        // ------------------------- SIDEBAR -------------------------

        private void HamburgerBtn_Click(object sender, RoutedEventArgs e)
        {
            LeftColumn.Width = new GridLength(IsPaneCollapsed ? 200 : 64);
            IsPaneCollapsed = !IsPaneCollapsed;
        }

        private void RightFrame_Navigated(object sender, NavigationEventArgs e) => UpdateSidebar(e.Content);

        /// <summary>
        /// Shows the previous, current and next page of the flow. Setup and its edit pages are
        /// outside the flow: the current marker moves down to Setup, and on top stays the flow
        /// page the user came from (to go back with one click) with its next page dimmed.
        /// </summary>
        private void UpdateSidebar(object content)
        {
            if (content is IFlowPage page)
            {
                _flowPage = page;
                _flowCommittee = Settings.Default.SelCommName;

                ShowLink(PreviousBtn, PreviousIcon, PreviousLabel, page.Previous, "back");
                ShowCurrent(page.TitleKey);
                ShowLink(NextBtn, NextIcon, NextLabel, page.Next, "next");
                ShowSetupAsCurrent(false);
            }
            else
            {
                var origin = _flowPage;
                var back = origin == null ? null
                    : new FlowLink(origin.TitleKey, new RelayCommand(() => NavigateRightFrame((Page)origin)));

                ShowLink(PreviousBtn, PreviousIcon, PreviousLabel, back, "back");
                CurrentBtn.Visibility = Visibility.Collapsed;
                ShowLink(NextBtn, NextIcon, NextLabel, origin?.Next is { } next ? next with { Command = null } : null, "next");
                ShowSetupAsCurrent(true);
            }
        }

        private static void ShowLink(Button button, SvgViewbox icon, TextBlock label, FlowLink? link, string arrowIcon)
        {
            button.Visibility = link == null ? Visibility.Collapsed : Visibility.Visible;
            if (link == null)
                return;

            icon.Source = IconUri(link.IsHome ? "house" : arrowIcon);
            label.SetResourceReference(TextBlock.TextProperty, link.LabelKey);
            button.SetResourceReference(TagProperty, link.LabelKey); // tooltip when collapsed
            button.Command = link.Command;

            // Without a command the link is dimmed: that page comes by itself
            button.IsEnabled = link.Command != null;
        }

        private void ShowCurrent(string titleKey)
        {
            CurrentBtn.Visibility = Visibility.Visible;
            CurrentIcon.Source = IconUri(titleKey == FlowLink.HomeKey ? "house" : "current");
            CurrentLabel.SetResourceReference(TextBlock.TextProperty, titleKey);
        }

        private void ShowSetupAsCurrent(bool isCurrent)
        {
            if (isCurrent)
                ConfigureBtn.SetResourceReference(BackgroundProperty, "SidebarActiveBrush");
            else
                ConfigureBtn.ClearValue(BackgroundProperty);

            ConfigureBtn.IsHitTestVisible = !isCurrent;
            ConfigureIcon.Source = IconUri(isCurrent ? "current" : "configure");
        }

        private static Uri IconUri(string name) => new($"pack://application:,,,/Resources/Icons/{name}.svg");

        private void ConfigureBtn_Click(object sender, RoutedEventArgs e) =>
            NavigateRightFrame(new SetupPage());

        private void OpenRollCall()
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
