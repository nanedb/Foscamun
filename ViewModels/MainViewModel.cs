using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using Foscamun.Data;

namespace Foscamun.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly SqliteDataAccess _dataAccess;

        public MainViewModel(SqliteDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }
    }
}
