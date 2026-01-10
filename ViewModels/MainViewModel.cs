using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using Foscamun2026.Data;

namespace Foscamun2026.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly SqliteDataAccess _dataAccess;

        public MainViewModel(SqliteDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        //[ObservableProperty]
        //private string welcomeMessage = "Benvenuto in Foscamun2026!";

        //[RelayCommand]
        //private void ShowMessage()
        //{
        //    MessageBox.Show("Hai cliccato il pulsante!");
        //}

        //[RelayCommand]
        //private async Task LoadData()
        //{
        //    var committees = await _dataAccess.GetCommitteesAsync();
        //    MessageBox.Show($"Trovati {committees.Count} comitati!");
        //}
    }
}