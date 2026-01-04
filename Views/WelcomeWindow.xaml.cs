using Foscamun2026.ViewModels;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace Foscamun2026.Views
{
    public partial class WelcomeWindow : Window
    {
        public WelcomeWindow()
        {
            InitializeComponent();
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ConfigureBtn_Click(object sender, RoutedEventArgs e)
        {
            // Nascondi la finestra corrente
            this.Hide();

            // Apri SetupWindow
            var setup = new SetupWindow();
            setup.Closed += (s, args) =>
            {
                // Quando SetupWindow viene chiusa, riapri WelcomeWindow
                this.Show();
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
    }
}
