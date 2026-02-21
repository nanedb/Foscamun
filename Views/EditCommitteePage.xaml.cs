using Foscamun2026.Data;
using Foscamun2026.Models;
using Foscamun2026.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Foscamun2026.Views
{
    public partial class EditCommitteePage : Page
    {
        private AddCommitteeViewModel VM => (AddCommitteeViewModel)DataContext;

        public EditCommitteePage(SqliteDataAccess db)
        {
            InitializeComponent();
            DataContext = new AddCommitteeViewModel(db);
        }

        public EditCommitteePage(SqliteDataAccess db, Committee committee)
        {
            InitializeComponent();
            DataContext = new AddCommitteeViewModel(db, committee);
        }

        private void AllCountries_Click(object sender, MouseButtonEventArgs e)
        {
            if (VM.SelectedCountry is Country c)
                VM.AllCountriesClicked(c);
        }

        private void SelectedCountries_Click(object sender, MouseButtonEventArgs e)
        {
            if (VM.SelectedCountry is Country c)
                VM.SelectedCountriesClicked(c);
        }
    }
}