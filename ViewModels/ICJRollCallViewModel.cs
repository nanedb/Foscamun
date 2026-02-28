using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Foscamun.Data;
using Foscamun.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Data;

namespace Foscamun.ViewModels
{
    /// <summary>
    /// ViewModel for ICJ roll call page.
    /// Manages present/absent members selection (advocates and jurors) before starting a session.
    /// </summary>
    public partial class ICJRollCallViewModel : ObservableObject
    {
        private readonly SqliteDataAccess _db;
        private readonly Action<string, string, string, string, int, List<ICJRollCallMember>> _navigateToSession;

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

        /// <summary>
        /// Gets the logo path for ICJ.
        /// Returns custom logo if exists, otherwise generic fallback.
        /// </summary>
        public string CommitteeLogoPath
        {
            get
            {
                string logoPath = Path.Combine(SqliteDataAccess.CommitteeLogoFolder, "ICJ.svg");

                if (File.Exists(logoPath))
                {
                    return logoPath;
                }

                return Path.Combine(SqliteDataAccess.CommitteeLogoFolder, "Generic.svg");
            }
        }

        public IRelayCommand ProceedCommand { get; }
        public IRelayCommand MarkAllPresentCommand { get; }

        public ICJRollCallViewModel(SqliteDataAccess db, Action<string, string, string, string, int, List<ICJRollCallMember>> navigateToSession)
        {
            _db = db;
            _navigateToSession = navigateToSession;

            for (int i = 1; i <= 10; i++)
            {
                Sessions.Add(i);
            }

            // Enable sorting by display name for both lists
            var viewAvailable = CollectionViewSource.GetDefaultView(AvailableMembers);
            viewAvailable.SortDescriptions.Add(
                new SortDescription(nameof(ICJRollCallMember.DisplayName), ListSortDirection.Ascending));

            var viewPresent = CollectionViewSource.GetDefaultView(PresentMembers);
            viewPresent.SortDescriptions.Add(
                new SortDescription(nameof(ICJRollCallMember.DisplayName), ListSortDirection.Ascending));

            ProceedCommand = new RelayCommand(Proceed, CanProceed);
            MarkAllPresentCommand = new RelayCommand(MarkAllPresent, CanMarkAllPresent);

            _ = LoadICJDataAsync();
        }

        /// <summary>
        /// Loads ICJ configuration and all members (advocates and jurors) from database.
        /// </summary>
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
            MarkAllPresentCommand.NotifyCanExecuteChanged();
        }

        public void PresentMembersClicked(ICJRollCallMember member)
        {
            PresentMembers.Remove(member);
            AvailableMembers.Add(member);
            ProceedCommand.NotifyCanExecuteChanged();
            MarkAllPresentCommand.NotifyCanExecuteChanged();
        }

        private bool CanMarkAllPresent()
        {
            return AvailableMembers.Count > 0;
        }

        private void MarkAllPresent()
        {
            var membersToMove = AvailableMembers.ToList();
            foreach (var member in membersToMove)
            {
                AvailableMembers.Remove(member);
                PresentMembers.Add(member);
            }
            ProceedCommand.NotifyCanExecuteChanged();
            MarkAllPresentCommand.NotifyCanExecuteChanged();
        }

        private bool CanProceed()
        {
            return PresentMembers.Count > 0;
        }

        private void Proceed()
        {
            var presentMembersList = PresentMembers.ToList();
            _navigateToSession(Judge, ViceJudge1, ViceJudge2, Topic, SelectedSession, presentMembersList);
        }
    }
}
