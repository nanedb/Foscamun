using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;

namespace Foscamun2026.Views
{
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
            this.Loaded += Home_Loaded;
        }

        private void Home_Loaded(object sender, RoutedEventArgs e)
        {
            // Sposta il focus sul primo elemento utile per accessibilità
            MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        public void Refresh()
        {
            // Aggiorna dati o UI se necessario
        }
    }
}
