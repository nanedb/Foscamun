using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Foscamun.Data;
using Foscamun.Helpers;
using Foscamun.ViewModels;
using Foscamun.Views;
using Foscamun.Properties;

namespace Foscamun
{
    public partial class App : Application
    {
        public IServiceProvider? Services { get; private set; }

        public static event Action? LanguageChanged;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Inizializza il database (crea le cartelle e il file .db se non esistono)
            var db = new SqliteDataAccess();

            // Copia i loghi delle commissioni se non esistono
            LogoInstaller.EnsureLogosInstalled(SqliteDataAccess.CommitteeLogoFolder);

            var lang = Settings.Default.Lang;
            if (string.IsNullOrEmpty(lang))
                lang = "en"; // default

            ChangeLanguage(lang);

            var services = new ServiceCollection();

            // Registrazioni
            services.AddSingleton<SqliteDataAccess>();
            services.AddSingleton<CommitteeListViewModel>();

            Services = services.BuildServiceProvider();

            // Correzione: Accesso a Properties.Settings.Default.Year
            Application.Current.Resources["YearResource"] = Settings.Default.Year;

            Settings.Default.PropertyChanged += (s, ev) =>
            {
                if (ev.PropertyName == "Year")
                    Application.Current.Resources["YearResource"] = Settings.Default.Year;
            };

            // Mostra MainWindow come prima finestra
            var welcomeWindow = new MainWindow();
            welcomeWindow.Show();
        }

        public static void ChangeLanguage(string langCode)
        {
            // Normalizza il codice lingua (es. "en-US" -> "en")
            if (langCode.Contains("-"))
                langCode = langCode.Split('-')[0];

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

            // ?? Notifica globale
            LanguageChanged?.Invoke();
        }
    }
}

