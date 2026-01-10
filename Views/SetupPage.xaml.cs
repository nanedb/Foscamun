#nullable enable
using Foscamun2026.Data;
using Foscamun2026.Models;
using Foscamun2026.ViewModels;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace Foscamun2026
{
    public partial class SetupPage : Page
    {
        public event Action? RequestClose;

        public SetupPage()
        {
            InitializeComponent();

            // MVVM: collega il ViewModel
            DataContext = new CommitteeListViewModel(new SqliteDataAccess());
            var VM = (CommitteeListViewModel)DataContext;

            // Carica i comitati
            _ = VM.LoadCommitteesAsync();

            // Imposta la lingua salvata
            string lang = Properties.Settings.Default.Lang;

            switch (lang)
            {
                case "fr-FR":
                    FraLangButton.IsChecked = true;
                    break;

                case "es-ES":
                    EspLangButton.IsChecked = true;
                    break;

                default:
                    EngLangButton.IsChecked = true;
                    break;
            }

            // Ordina la lista
            CommitteesListBox.Items.SortDescriptions.Clear();
            CommitteesListBox.Items.SortDescriptions.Add(
                new SortDescription("Name", ListSortDirection.Ascending));
        }

        // Assumiamo che gli elementi siano oggetti Committee con proprietà CommID e Name
        private void SelectSavedCommitteeFromExistingItems()
        {
            // salva l'id che hai memorizzato
            int savedId = Properties.Settings.Default.SelCommID;

            // prendi la sorgente dati corrente (se ItemsSource è IEnumerable<Committee>)
            var items = CommitteesListBox.ItemsSource as IEnumerable<Committee>;
            if (items == null)
            {
                // fallback: se ItemsSource non è impostato, prova a leggere Items (container)
                items = CommitteesListBox.Items.Cast<Committee>();
            }

            // ordina per nome e crea lista indicizzata
            var ordered = items.OrderBy(c => c.Name, StringComparer.CurrentCulture).ToList();

            // trova l'indice dell'elemento con l'ID salvato
            int index = ordered.FindIndex(c => c.CommID == savedId);

            if (index >= 0)
            {
                // seleziona l'elemento corretto
                CommitteesListBox.SelectedItem = ordered[index];
                CommitteesListBox.ScrollIntoView(CommitteesListBox.SelectedItem);
            }
            else
            {
                CommitteesListBox.SelectedIndex = -1;
            }
        }

        private void SetupPage_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                SelectSavedCommitteeFromExistingItems();
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        // ------------------------------------------------------------
        // CAMBIO LINGUA (NON DISTRUGGE GLI STILI GLOBALI)
        // ------------------------------------------------------------
        private void ChangeLanguage(string culture)
        {
            // Imposta la cultura del thread
            var cultureInfo = CultureInfo.GetCultureInfo(culture);
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;

            // Trova il dizionario Strings attuale
            var oldDict = Application.Current.Resources.MergedDictionaries
                .FirstOrDefault(d => d.Source != null &&
                                     d.Source.OriginalString.Contains("Strings"));

            // Crea il nuovo dizionario
            var newDict = new ResourceDictionary
            {
                Source = culture switch
                {
                    "fr-FR" => new Uri("Strings/Strings.fr.xaml", UriKind.Relative),
                    "es-ES" => new Uri("Strings/Strings.es.xaml", UriKind.Relative),
                    _ => new Uri("Strings/Strings.en.xaml", UriKind.Relative)
                }
            };

            // Sostituisci SOLO il dizionario delle stringhe
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

        // Eventi RadioButton
        private void EngLangButton_Checked(object sender, RoutedEventArgs e) => ChangeLanguage("en-US");
        private void FraLangButton_Checked(object sender, RoutedEventArgs e) => ChangeLanguage("fr-FR");
        private void EspLangButton_Checked(object sender, RoutedEventArgs e) => ChangeLanguage("es-ES");


        // ------------------------------------------------------------
        // SELEZIONE COMITATO
        // ------------------------------------------------------------
        private void CommitteesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CommitteesListBox.SelectedItem is Committee selected)
            {
                Properties.Settings.Default.SelCommID = selected.CommID;
                Properties.Settings.Default.SelCommName = selected.Name;
                Properties.Settings.Default.Save();
            }
        }

        // ------------------------------------------------------------
        // AGGIUNGI COMITATO
        // ------------------------------------------------------------
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            //var addWindow = new AddCommitteeWindow { Owner = this };

            //this.Hide();
            //bool? result = addWindow.ShowDialog();
            //this.Show();

            //if (result == true && DataContext is CommitteeListViewModel vm)
            //    _ = vm.LoadCommitteesAsync();
        }

        // ------------------------------------------------------------
        // MODIFICA COMITATO
        // ------------------------------------------------------------
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            //if (CommitteesListBox.SelectedItem is not Committee selected)
            //{
            //    MessageBox.Show("Select a committee first.");
            //    return;
            //}

            //var editWindow = new EditCommitteeWindow(selected) { Owner = this };

            //this.Hide();
            //bool? result = editWindow.ShowDialog();
            //this.Show();

            //if (result == true && DataContext is CommitteeListViewModel vm)
            //    _ = vm.LoadCommitteesAsync();
        }

        // ------------------------------------------------------------
        // RIMUOVI COMITATO
        // ------------------------------------------------------------
        private async void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (CommitteesListBox.SelectedItem is not Committee selected)
            {
                MessageBox.Show("Select a committee first.");
                return;
            }

            if (MessageBox.Show($"Remove committee '{selected.Name}'?",
                "Foscamun", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                var data = new SqliteDataAccess();
                await data.RemoveCommitteeAsync(selected);

                if (DataContext is CommitteeListViewModel vm)
                    _ = vm.LoadCommitteesAsync();
            }
        }

        // ------------------------------------------------------------
        // NAVIGAZIONE
        // ------------------------------------------------------------
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            //if (CommitteesListBox.SelectedItem is not Committee selected)
            //{
            //    MessageBox.Show("Select a committee first.");
            //    return;
            //}

            //Window next = selected.Name == "ICJ"
            //    ? new ICJStartWindow { Owner = this.Owner }
            //    : new StartWindow { Owner = this.Owner };

            //Properties.Settings.Default.Save();

            //this.Hide();
            //next.ShowDialog();
            //this.Show();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            RequestClose?.Invoke();

            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }
    }
}