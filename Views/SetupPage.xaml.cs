using Foscamun2026.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Foscamun2026.Views
{
    public partial class SetupPage : Page
    {
        public SetupPage()
        {
            InitializeComponent();

            // Se il DataContext non è già stato impostato da fuori, lo creo qui
            if (DataContext is not SetupPageViewModel vm)
            {
                vm = new SetupPageViewModel();
                DataContext = vm;
            }

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
            CommitteesListBox.Focus();

            // Scroll dopo che la UI ha finito di disegnare
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var vm = DataContext as SetupPageViewModel;
                if (vm?.SelectedCommittee != null)
                {
                    CommitteesListBox.UpdateLayout();
                    CommitteesListBox.ScrollIntoView(vm.SelectedCommittee);
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