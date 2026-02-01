using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Foscamun2026.Data;
using Foscamun2026.Models;
using Foscamun2026.Views;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Foscamun2026.ViewModels
{
    public partial class AddICJMemberViewModel : ObservableObject
    {
        private readonly SqliteDataAccess _db;

        // -------------------------
        //  PROPRIETÀ
        // -------------------------

        [ObservableProperty]
        private string jurorName = string.Empty;

        [ObservableProperty]
        private string lawyerName = string.Empty;

        public ObservableCollection<ICJMember> Members { get; } = new();

        [ObservableProperty]
        private ICJMember? selectedMember;

        // -------------------------
        //  COSTRUTTORE
        // -------------------------

        public AddICJMemberViewModel(SqliteDataAccess db)
        {
            _db = db;
            _ = LoadMembersAsync();
        }

        // -------------------------
        //  CARICAMENTO MEMBRI
        // -------------------------

        private async Task LoadMembersAsync()
        {
            Members.Clear();
            var list = await _db.LoadICJMembersAsync();
            foreach (var m in list)
                Members.Add(m);
        }

        // -------------------------
        //  COMANDI
        // -------------------------

        [RelayCommand]
        private async Task AddJurorAsync()
        {
            if (string.IsNullOrWhiteSpace(JurorName))
                return;

            await _db.InsertICJMemberAsync(
                name: JurorName,
                kind: "Juror",
                isoCode: "JU"
            );

            JurorName = string.Empty;
            await LoadMembersAsync();
        }

        [RelayCommand]
        private async Task AddLawyerAsync()
        {
            if (string.IsNullOrWhiteSpace(LawyerName))
                return;

            await _db.InsertICJMemberAsync(
                name: LawyerName,
                kind: "Lawyer",
                isoCode: "LW"
            );

            LawyerName = string.Empty;
            await LoadMembersAsync();
        }

        [RelayCommand]
        private async Task RemoveMemberAsync()
        {
            if (SelectedMember is null)
                return;

            await _db.DeleteICJMemberAsync(SelectedMember.MemberID);
            await LoadMembersAsync();
        }

        [RelayCommand]
        private void Back()
        {
            MainWindow.Instance.NavigateRightFrame(new AddICJPage(_db));
        }
    }
}