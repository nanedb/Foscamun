using Foscamun2026.Models;
using Foscamun2026.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Foscamun2026.Views
{
    public partial class AddCommitteePage : Page
    {
        public AddCommitteePage()
        {
            InitializeComponent();
        }

        private AddCommitteeViewModel VM => (AddCommitteeViewModel)DataContext;

        // -------------------------
        //  CLICK SU LISTA SINISTRA
        // -------------------------
        private void AllCountries_Click(object sender, MouseButtonEventArgs e)
        {
            if (VM.SelectedCountry is Country c)
                VM.AllCountriesClicked(c);
        }

        // -------------------------
        //  CLICK SU LISTA DESTRA
        // -------------------------
        private void SelectedCountries_Click(object sender, MouseButtonEventArgs e)
        {
            if (VM.SelectedCountry is Country c)
                VM.SelectedCountriesClicked(c);
        }
    }
}