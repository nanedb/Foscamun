using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Foscamun2026.Data;
using Foscamun2026.ViewModels;
using Foscamun2026.Views;

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

            // Mostra WelcomeWindow come prima finestra
            var welcomeWindow = new WelcomeWindow();
            welcomeWindow.Show();
        }
    }
}

