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

        public MainWindow()
        {
            OpenRollCallCommand = new RelayCommand(OpenRollCall, () => IsRollCallEnabled);
            GoHomeCommand = new RelayCommand(() =>
            {
                if (ConfirmDiscardChanges())
                    NavigateRightFrame(new HomePage());
            });

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
            if (e.PropertyName == nameof(Settings.Default.SelCommName))
                OpenRollCallCommand.NotifyCanExecuteChanged();
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
        /// outside the flow: the current marker moves down to Setup, on top go Home and the
        /// roll call, which only Setup can open (the edit pages save or cancel first).
        /// </summary>
        private void UpdateSidebar(object content)
        {
            if (content is IFlowPage page)
            {
                ShowLink(PreviousBtn, PreviousIcon, PreviousLabel, page.Previous, "back");
                ShowCurrent(page.TitleKey);
                ShowLink(NextBtn, NextIcon, NextLabel, page.Next, "next");
                ShowSetupAsCurrent(false);
            }
            else
            {
                ShowLink(PreviousBtn, PreviousIcon, PreviousLabel, new FlowLink(FlowLink.HomeKey, GoHomeCommand), "back");
                CurrentBtn.Visibility = Visibility.Collapsed;
                ShowLink(NextBtn, NextIcon, NextLabel,
                    new FlowLink("RollCallBtn", content is SetupPage ? OpenRollCallCommand : null), "next");
                ShowSetupAsCurrent(true);
            }
        }

        /// <summary>
        /// On a page with unsaved changes, asks before leaving it. True to go on.
        /// </summary>
        private bool ConfirmDiscardChanges()
        {
            if (RightFrame.Content is not IUnsavedChanges { HasUnsavedChanges: true })
                return true;

            return AppDialog.Show(AppDialog.Text("UnsavedChangesMsg"), AppDialog.Text("UnsavedChangesTitle"),
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
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
                AppDialog.Show(AppDialog.Text("NoCommitteeMsg"), AppDialog.Text("NoCommitteeTitle"),
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
                AppDialog.Show(AppDialog.Text("CommitteeNotFoundMsg", name), AppDialog.Text("ErrorTitle"),
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            NavigateRightFrame(new CommitteeRollCallPage(committee, Db));
        }

        // Exit and the window's X take the same way out, through OnClosing
        private void ExitBtn_Click(object sender, RoutedEventArgs e) => Close();

        private bool _closingForUpdate;

        /// <summary>Closes the app without questions, so that the update can replace its files.</summary>
        public static void CloseForUpdate()
        {
            Instance._closingForUpdate = true;
            Instance.Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (e.Cancel || _closingForUpdate)
                return;

            // Unsaved changes: that question is enough, no second one to exit
            if (RightFrame.Content is IUnsavedChanges { HasUnsavedChanges: true })
            {
                e.Cancel = !ConfirmDiscardChanges();
                return;
            }

            if (!Settings.Default.ConfirmExit)
                return;

            var app = Application.Current;
            var answer = AppDialog.Show(
                (string)app.FindResource("ExitConfirmMessage"),
                (string)app.FindResource("ExitConfirmTitle"),
                MessageBoxButton.YesNo, MessageBoxImage.Question,
                (string)app.FindResource("DontAskAgain"), out bool dontAskAgain);

            if (answer != MessageBoxResult.Yes)
            {
                e.Cancel = true;
                return;
            }

            // Setup can turn the question back on
            if (dontAskAgain)
            {
                Settings.Default.ConfirmExit = false;
                Settings.Default.Save();
            }
        }

        // ------------------- ANIMATED NAVIGATION -------------------

        private bool _isTransitioning;        // a slide-out is running
        private bool _navigatingAnimated;     // the navigation comes from NavigateRightFrame

        /// <summary>Slides the current page out, navigates, then slides the new page in.</summary>
        public void NavigateRightFrame(Page page)
        {
            // A second request during the slide-out (a double click) is ignored
            if (_isTransitioning)
                return;
            _isTransitioning = true;

            var slideOut = (Storyboard)Resources["RightFrameSlideOut"];

            slideOut.Completed -= SlideOutCompleted;
            slideOut.Completed += SlideOutCompleted;
            slideOut.Begin();

            void SlideOutCompleted(object? s, EventArgs e)
            {
                slideOut.Completed -= SlideOutCompleted;
                _navigatingAnimated = true;
                try { RightFrame.Navigate(page); }
                finally { _navigatingAnimated = false; }
                _isTransitioning = false;
                ((Storyboard)Resources["RightFrameSlideIn"]).Begin();
            }
        }

        /// <summary>
        /// The flow pages navigate through their own NavigationService: those navigations
        /// are stopped here and taken again through <see cref="NavigateRightFrame"/>, so that
        /// every page change is animated. The first page and Back navigations go straight.
        /// </summary>
        private void RightFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (_navigatingAnimated || RightFrame.Content == null ||
                e.NavigationMode != NavigationMode.New || e.Content is not Page page)
                return;

            e.Cancel = true;
            NavigateRightFrame(page);
        }

        // --------------------------- MISC ---------------------------

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        protected override void OnClosed(EventArgs e)
        {
            Settings.Default.PropertyChanged -= Settings_PropertyChanged;
            base.OnClosed(e);

            // Other windows (the timer) close with the app
            Application.Current.Shutdown();
        }
    }
}
