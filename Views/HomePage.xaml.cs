using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

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
            // Accessibilità
            MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

            // Avvia il parallax del banner
            var sb = (Storyboard)FindResource("BannerParallaxIn");
            sb.Begin();
        }

        public void Refresh()
        {
            // Aggiorna dati o UI se necessario
        }
    }
}
