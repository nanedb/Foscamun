using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        // Handler chiamato dal Click dell'hamburger (definito in XAML)
        private void HamburgerBtn_Click(object sender, RoutedEventArgs e)
        {
            TogglePane();
        }

        private void TogglePane()
        {
            const double expandedWidth = 150;
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

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ConfigureBtn_Click(object sender, RoutedEventArgs e)
        {
            // Forza lo stato Normal sul pulsante che è stato cliccato
            if (sender is Control clickedCtrl)
            {
                VisualStateManager.GoToState(clickedCtrl, "Normal", true);
            }

            // Nascondi la finestra corrente
            this.Hide();

            // Apri SetupWindow
            var setup = new SetupWindow();
            setup.Closed += (s, args) =>
            {
                // Quando SetupWindow viene chiusa, riapri MainWindow
                this.Show();

                // Ripristina lo stato Normal su tutti i pulsanti della finestra (evita hover "bloccati")
                //ResetAllMenuButtonsToNormal();
            };
            setup.ShowDialog();
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
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // Ripristina lo stato Normal su tutti i Button nella finestra
        //private void ResetAllMenuButtonsToNormal()
        //{
        //    foreach (var btn in FindVisualChildren<Button>(this))
        //    {
        //        // Se vuoi limitare ai soli pulsanti con uno specifico Style, decommenta e adatta:
        //        // if (btn.Style == (Style)FindResource("MenuButtonTest"))
        //        VisualStateManager.GoToState(btn, "Normal", true);
        //    }
        //}

        // Helper per enumerare la visual tree
        //private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        //{
        //    if (depObj == null) yield break;
        //    for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
        //    {
        //        var child = VisualTreeHelper.GetChild(depObj, i);
        //        if (child is T t) yield return t;
        //        foreach (var childOfChild in FindVisualChildren<T>(child)) yield return childOfChild;
        //    }
        //}
    }
}
