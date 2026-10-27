using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Foscamun.Views
{
    public partial class HomePage : Page, IFlowPage
    {
        // Flow: Home is the first page, next comes the roll call
        public string TitleKey => FlowLink.HomeKey;
        public FlowLink? Previous => null;
        public FlowLink? Next => new("RollCallBtn", MainWindow.Instance.OpenRollCallCommand);

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
