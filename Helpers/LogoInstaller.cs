using System;
using System.IO;
using System.Reflection;
using System.Windows;

namespace Foscamun.Helpers
{
    public static class LogoInstaller
    {
        /// <summary>
        /// Copia i loghi delle commissioni dalle risorse embedded alla cartella AppData
        /// se non esistono già. Questo metodo va chiamato al primo avvio dell'applicazione.
        /// </summary>
        public static void EnsureLogosInstalled(string committeeLogoFolder)
        {
            try
            {
                // Lista dei loghi da copiare (aggiungi tutti i nomi delle commissioni)
                string[] committeeLogos = new[]
                {
                    "ICJ.svg",
                    // Aggiungi qui altri nomi di file logo
                    // "ECOSOC.svg",
                    // "UNHRC.svg",
                    // ecc.
                };

                var assembly = Assembly.GetExecutingAssembly();
                var resourcePrefix = "Foscamun2026.Resources.Committee Logo.";

                foreach (var logoFileName in committeeLogos)
                {
                    string destinationPath = Path.Combine(committeeLogoFolder, logoFileName);

                    // Salta se il file esiste già
                    if (File.Exists(destinationPath))
                        continue;

                    // Costruisci il nome della risorsa embedded
                    string resourceName = resourcePrefix + logoFileName;

                    // Cerca la risorsa embedded
                    using var resourceStream = assembly.GetManifestResourceStream(resourceName);

                    if (resourceStream == null)
                    {
                        // Risorsa non trovata, salta
                        continue;
                    }

                    // Copia il file nella cartella di destinazione
                    using var fileStream = File.Create(destinationPath);
                    resourceStream.CopyTo(fileStream);
                }
            }
            catch (Exception ex)
            {
                // Log dell'errore (opzionale)
                MessageBox.Show(
                    $"Error installing committee logos: {ex.Message}",
                    "Installation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
            }
        }
    }
}
