using Foscamun.Data;
using Foscamun.Models;
using Foscamun.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Foscamun.Views
{
    public partial class CommitteeEditPage : Page
    {
        private CommitteeEditViewModel VM => (CommitteeEditViewModel)DataContext;

        public CommitteeEditPage(SqliteDataAccess db)
        {
            InitializeComponent();
            DataContext = new CommitteeEditViewModel(db);
        }

        public CommitteeEditPage(SqliteDataAccess db, Committee committee)
        {
            InitializeComponent();
            DataContext = new CommitteeEditViewModel(db, committee);
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
