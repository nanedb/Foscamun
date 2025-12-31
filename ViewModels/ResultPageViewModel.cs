using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Foscamun2026.Models;
using Foscamun2026.Data;

namespace Foscamun2026.ViewModels
{
    public partial class ResultPageViewModel : ObservableObject
    {
        private readonly SqliteDataAccess _dataAccess;

        [ObservableProperty]
        private ObservableCollection<VoteResult> results = new();

        public ResultPageViewModel(SqliteDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        [RelayCommand]
        public async Task LoadResultsAsync()
        {
            var list = await _dataAccess.GetResultsAsync();
            Results = new ObservableCollection<VoteResult>(list);
        }
    }
}
