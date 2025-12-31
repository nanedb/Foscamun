using System;
using Microsoft.Data.Sqlite;
using System.Linq;
using System.Windows;
using System.Collections.Generic;


namespace Foscamun2026
{
    public partial class SetupWindow : Window
    {
        public SetupWindow()
        {
            InitializeComponent();
            LoadCommittees();
        }

        private void LoadCommittees()
        {
            try
            {
                // Percorso al DB nella root del progetto
                string dbPath = "Data Source=foscamun.db";

                using (var conn = new SqliteConnection(dbPath))
                {
                    conn.Open();

                    string sql = "SELECT Name FROM Committees"; // tabella Committees
                    using (var cmd = new SqliteCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        var committees = new List<string>();
                        while (reader.Read())
                        {
                            committees.Add(reader.GetString(0)); // seconda colonna: Name
                        }
                        CommitteeList.ItemsSource = committees;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errore nel caricamento delle commissioni: " + ex.Message);
            }
        }

        private void LanguageSelector_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (LanguageSelector.SelectedItem is System.Windows.Controls.ComboBoxItem item)
            {
                string? culture = item.Tag as string;
                if (!string.IsNullOrEmpty(culture))
                {
                    ChangeLanguage(culture);
                }
            }
        }

        private void ChangeLanguage(string culture)
        {
            // Trova il dizionario delle stringhe già caricato
            var oldDict = Application.Current.Resources.MergedDictionaries
                .FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Contains("Strings"));

            // Crea il nuovo dizionario
            var newDict = new ResourceDictionary();
            switch (culture)
            {
                case "fr-FR":
                    newDict.Source = new Uri("Resources/Strings.fr.xaml", UriKind.Relative);
                    break;
                case "es-ES":
                    newDict.Source = new Uri("Resources/Strings.es.xaml", UriKind.Relative);
                    break;
                default:
                    newDict.Source = new Uri("Resources/Strings.en.xaml", UriKind.Relative);
                    break;
            }

            // Sostituisci solo il dizionario delle stringhe
            if (oldDict != null)
            {
                int index = Application.Current.Resources.MergedDictionaries.IndexOf(oldDict);
                Application.Current.Resources.MergedDictionaries[index] = newDict;
            }
            else
            {
                Application.Current.Resources.MergedDictionaries.Add(newDict);
            }

            // Salva preferenza
            Properties.Settings.Default.Lang = culture;
            Properties.Settings.Default.Save();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            // Chiude la finestra SetupWindow
            this.Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
