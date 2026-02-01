using Foscamun2026.Data;
using Foscamun2026.Models;
using Foscamun2026.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Foscamun2026.Views
{
    public partial class SetupPage : Page
    {
        private readonly SetupPageViewModel _vm;
        private readonly SqliteDataAccess _db;

        public SetupPage()
        {
            InitializeComponent();

            _db = ((App)Application.Current).Services!.GetRequiredService<SqliteDataAccess>();
            _vm = new SetupPageViewModel(_db);
            DataContext = _vm;

            Loaded += SetupPage_Loaded;

            var lang = Properties.Settings.Default.Lang;

            switch (lang)
            {
                case "en": EngLangButton.IsChecked = true; break;
                case "fr": FraLangButton.IsChecked = true; break;
                case "es": EspLangButton.IsChecked = true; break;
            }
        }

        private void SetupPage_Loaded(object sender, RoutedEventArgs e)
        {
            CommitteesListBox.Focus();

            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (_vm.SelectedCommittee != null)
                {
                    CommitteesListBox.UpdateLayout();
                    CommitteesListBox.ScrollIntoView(_vm.SelectedCommittee);
                }
            }), DispatcherPriority.Background);
        }

        // -------------------------
        //  ADD
        // -------------------------
        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var page = new AddCommitteePage(_db);

            MainWindow.Instance.NavigateRightFrame(page);
        }

        // -------------------------
        //  EDIT
        // -------------------------
        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_vm.SelectedCommittee == null)
            {
                MessageBox.Show("Please select a committee to edit.");
                return;
            }

            var page = new AddCommitteePage(_db, _vm.SelectedCommittee);

            MainWindow.Instance.NavigateRightFrame(page);
        }

        // -------------------------
        //  DELETE
        // -------------------------
        private async void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_vm.SelectedCommittee == null)
            {
                MessageBox.Show("Please select a committee to delete.");
                return;
            }

            var result = MessageBox.Show(
                $"Are you sure you want to delete '{_vm.SelectedCommittee.Name}'?",
                "Confirm delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            await _db.RemoveCommitteeAsync(_vm.SelectedCommittee);

            _vm.Committees.Remove(_vm.SelectedCommittee);
            _vm.SelectedCommittee = null;
        }

        // -------------------------
        //  LANGUAGE SWITCH
        // -------------------------
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