using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Foscamun2026.Data;
using Foscamun2026.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Data;

namespace Foscamun2026.ViewModels
{
    public partial class ICJRollCallViewModel : ObservableObject
    {
        private readonly SqliteDataAccess _db;

        [ObservableProperty]
        private string judge = "";

        [ObservableProperty]
        private string viceJudge1 = "";

        [ObservableProperty]
        private string viceJudge2 = "";

        [ObservableProperty]
        private string topic = "";

        [ObservableProperty]
        private int selectedSession = 1;

        [ObservableProperty]
        private ICJRollCallMember? selectedMember;

        public ObservableCollection<int> Sessions { get; } = new();

        public ObservableCollection<ICJRollCallMember> AvailableMembers { get; } = new();

        public ObservableCollection<ICJRollCallMember> PresentMembers { get; } = new();

        public string CommitteeLogoPath => "pack://application:,,,/Resources/Committee Logo/ICJ.svg";

        public IRelayCommand ProceedCommand { get; }

        public ICJRollCallViewModel(SqliteDataAccess db)
        {
            _db = db;

            for (int i = 1; i <= 10; i++)
            {
                Sessions.Add(i);
            }

            // Sorting per entrambe le liste
            var viewAvailable = CollectionViewSource.GetDefaultView(AvailableMembers);
            viewAvailable.SortDescriptions.Add(
                new SortDescription(nameof(ICJRollCallMember.DisplayName), ListSortDirection.Ascending));

            var viewPresent = CollectionViewSource.GetDefaultView(PresentMembers);
            viewPresent.SortDescriptions.Add(
                new SortDescription(nameof(ICJRollCallMember.DisplayName), ListSortDirection.Ascending));

            ProceedCommand = new RelayCommand(Proceed, CanProceed);

            _ = LoadICJDataAsync();
        }

        private async Task LoadICJDataAsync()
        {
            try
            {
                var icj = _db.ICJRepository.Load();

                if (icj != null)
                {
                    Judge = icj.Judge;
                    ViceJudge1 = icj.ViceJudge1;
                    ViceJudge2 = icj.ViceJudge2;
                    Topic = icj.Topic;
                }

                // Carica tutti i membri ICJ (Advocates e Jurors)
                var members = await _db.LoadICJMembersAsync();

                AvailableMembers.Clear();
                foreach (var member in members)
                {
                    AvailableMembers.Add(new ICJRollCallMember(member));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR loading ICJ data: " + ex.Message);
            }
        }

        public void AvailableMembersClicked(ICJRollCallMember member)
        {
            AvailableMembers.Remove(member);
            PresentMembers.Add(member);
            ProceedCommand.NotifyCanExecuteChanged();
        }

        public void PresentMembersClicked(ICJRollCallMember member)
        {
            PresentMembers.Remove(member);
            AvailableMembers.Add(member);
            ProceedCommand.NotifyCanExecuteChanged();
        }

        private bool CanProceed()
        {
            return PresentMembers.Count > 0;
        }

        private void Proceed()
        {
            // TODO: Navigate to ICJ Session page
            Debug.WriteLine($"Proceeding with {PresentMembers.Count} present members in session {SelectedSession}");
        }
    }
}
