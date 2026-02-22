using CommunityToolkit.Mvvm.ComponentModel;
using Foscamun2026.Data;
using Foscamun2026.Models;

namespace Foscamun2026.ViewModels
{
    public partial class ICJRollCallViewModel : ObservableObject
    {
        private readonly SqliteDataAccess _db;

        [ObservableProperty]
        private string _judge = "";

        [ObservableProperty]
        private string _viceJudge1 = "";

        [ObservableProperty]
        private string _viceJudge2 = "";

        [ObservableProperty]
        private string _topic = "";

        public string CommitteeLogoPath => "pack://application:,,,/Resources/Committee Logo/ICJ.svg";

        public ICJRollCallViewModel(SqliteDataAccess db)
        {
            _db = db;
            LoadICJData();
        }

        private void LoadICJData()
        {
            var icj = _db.ICJRepository.Load();

            if (icj != null)
            {
                Judge = icj.Judge;
                ViceJudge1 = icj.ViceJudge1;
                ViceJudge2 = icj.ViceJudge2;
                Topic = icj.Topic;
            }
        }
    }
}