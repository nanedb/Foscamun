using Foscamun2026.Data;
using Foscamun2026.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Foscamun2026.Views
{
    public partial class SetupPage : Page
    {
        private SetupPageViewModel _vm;

        public SetupPage()
        {
            InitializeComponent();

            var db = ((App)Application.Current).Services!.GetRequiredService<SqliteDataAccess>();
            _vm = new SetupPageViewModel(db);
            DataContext = _vm;

            // Focus automatico sulla ListBox quando la pagina è caricata
            Loaded += SetupPage_Loaded;

            var lang = Properties.Settings.Default.Lang;

            switch (lang)
            {
                case "en": EngLangButton.IsChecked = true; break;
                case "fr": FraLangButton.IsChecked = true; break;
                case "es": EspLangButton.IsChecked = true; break;
            }
        }

        private void SetupPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // 🔥 ASSEGNA IL NAVIGATION SERVICE QUI
            _vm.NavigationService = NavigationService;

            CommitteesListBox.Focus();

            // Scroll dopo che la UI ha finito di disegnare
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (_vm.SelectedCommittee != null)
                {
                    CommitteesListBox.UpdateLayout();
                    CommitteesListBox.ScrollIntoView(_vm.SelectedCommittee);
                }
            }), DispatcherPriority.Background);
        }

        private void EngLangButton_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Lang = "en";
            Properties.Settings.Default.Save();
            App.ChangeLanguage("en");
        }
        private void FraLangButton_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Lang = "fr";
            Properties.Settings.Default.Save();
            App.ChangeLanguage("fr");
        }

        private void EspLangButton_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Lang = "es";
            Properties.Settings.Default.Save();
            App.ChangeLanguage("es");
        }
    }
}