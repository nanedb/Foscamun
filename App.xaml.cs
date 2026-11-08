using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Foscamun.Data;
using Foscamun.ViewModels;
using Foscamun.Views;
using Foscamun.Properties;
using Foscamun.Services;

namespace Foscamun
{
    public partial class App : Application
    {
        public IServiceProvider? Services { get; private set; }

        public static event Action? LanguageChanged;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Le impostazioni utente sono salvate per versione assembly: dopo un
            // aggiornamento vanno migrate dalla versione precedente.
            if (!Settings.Default.SettingsUpgraded)
            {
                Settings.Default.Upgrade();
                Settings.Default.SettingsUpgraded = true;
                Settings.Default.Save();
            }

            // Gli avvisi possono arrivare prima della finestra principale: chiuderli non deve chiudere l'app
            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            // La lingua prima di tutto: serve anche agli avvisi sul database
            var lang = Settings.Default.Lang;
            if (string.IsNullOrEmpty(lang))
                lang = "en"; // default

            ChangeLanguage(lang);

            // Il database esistente si tiene, a meno che non sia incompatibile con questa versione
            if (!DatabaseGuard.EnsureCompatible())
            {
                Shutdown();
                return;
            }

            // Inizializza il database (crea le cartelle e il file .db se non esistono, aggiorna lo schema)
            var db = new SqliteDataAccess();
            DatabaseGuard.StampVersion();

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

                // Changed from Setup's check box: keep it
                if (ev.PropertyName == "ConfirmExit")
                    Settings.Default.Save();
            };

            // Mostra MainWindow come prima finestra
            var welcomeWindow = new MainWindow();
            MainWindow = welcomeWindow;
            ShutdownMode = ShutdownMode.OnMainWindowClose;
            welcomeWindow.Show();

            // Nuove versioni su GitHub: il controllo non blocca l'avvio
            _ = UpdateService.CheckAtStartupAsync();
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

