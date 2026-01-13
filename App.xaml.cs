using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Foscamun2026.Data;
using Foscamun2026.ViewModels;
using Foscamun2026.Views;
using Foscamun2026.Properties;

namespace Foscamun2026
{
    public partial class App : Application
    {
        public IServiceProvider? Services { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();

            // Registrazioni
            services.AddSingleton<SqliteDataAccess>();
            services.AddSingleton<CommitteeListViewModel>();

            Services = services.BuildServiceProvider();

            // Correzione: Accesso a Properties.Settings.Default.Year
            Application.Current.Resources["YearResource"] = Foscamun2026.Properties.Settings.Default.Year;

            Foscamun2026.Properties.Settings.Default.PropertyChanged += (s, ev) =>
            {
                if (ev.PropertyName == "Year")
                    Application.Current.Resources["YearResource"] = Foscamun2026.Properties.Settings.Default.Year;
            };

            Foscamun2026.Properties.Settings.Default.PropertyChanged += (s, ev) =>
            {
                if (ev.PropertyName == "Year")
                    Application.Current.Resources["YearResource"] = Foscamun2026.Properties.Settings.Default.Year;
            };

            // Mostra MainWindow come prima finestra
            var welcomeWindow = new MainWindow();
            welcomeWindow.Show();
        }

        public static void ChangeLanguage(string langCode)
        {
            // Carica il nuovo dizionario
            var dict = new ResourceDictionary
            {
                Source = new Uri($"/Strings/Strings.{langCode}.xaml", UriKind.Relative)
            };

            // Rimuove il vecchio dizionario della lingua
            var oldDict = Application.Current.Resources.MergedDictionaries
                .FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Contains("/Strings/Strings."));

            if (oldDict != null)
                Application.Current.Resources.MergedDictionaries.Remove(oldDict);

            // Aggiunge il nuovo
            Application.Current.Resources.MergedDictionaries.Add(dict);

            // Salva nei Settings
            Settings.Default.Lang = langCode;
            Settings.Default.Save();
        }
    }
}

