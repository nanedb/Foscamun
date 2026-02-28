using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Foscamun.Data;
using Foscamun.Models;

namespace Foscamun.ViewModels
{
    public partial class CommitteeListViewModel : ObservableObject
    {
        private readonly SqliteDataAccess _dataAccess;

        [ObservableProperty]
        private ObservableCollection<Committee> committees = new();

        public CommitteeListViewModel(SqliteDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
            _ = LoadCommitteesAsync();
        }

        [RelayCommand]
        public async Task LoadCommitteesAsync()
        {
            var list = await _dataAccess.GetCommitteesAsync();
            Committees = new ObservableCollection<Committee>(list);
        }

        [RelayCommand]
        public async Task AddCommitteeAsync()
        {
            var newCommittee = new Committee
            {
                Name = "Nuovo Comitato",
                Topic = "Tema",
                President = "Presidente",
                VicePresident = "VicePresidente",
                Moderator = "Moderatore"
            };

            await _dataAccess.AddCommitteeAsync(newCommittee);
            await LoadCommitteesAsync();
        }
    }
}
