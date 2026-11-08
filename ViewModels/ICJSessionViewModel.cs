using CommunityToolkit.Mvvm.Input;
using Foscamun.Data;
using Foscamun.Models;
using System.Windows;
using System.Windows.Data;
using Foscamun.Views;

namespace Foscamun.ViewModels
{
    /// <summary>
    /// ViewModel for an ICJ session: the speakers are the present advocates and jurors,
    /// and only jurors vote. Lists, clock and warnings live in <see cref="SessionViewModelBase{TSpeaker}"/>.
    /// </summary>
    public partial class ICJSessionViewModel : SessionViewModelBase<ICJRollCallMember>
    {
        private readonly SqliteDataAccess _dataAccess;
        private readonly List<ICJRollCallMember> _presentMembers; // Immutable list of present members
        private readonly Action<List<ICJRollCallMember>> _navigateToVoting;

        public string Judge { get; }
        public string ViceJudge1 { get; }
        public string ViceJudge2 { get; }
        public string Topic { get; }

        public ICJSessionViewModel(string judge, string viceJudge1, string viceJudge2, string topic, int session, List<ICJRollCallMember> presentMembers, Action<List<ICJRollCallMember>> navigateToVoting, SqliteDataAccess dataAccess)
            : base(session, presentMembers)
        {
            _dataAccess = dataAccess;
            _presentMembers = presentMembers;
            _navigateToVoting = navigateToVoting;

            Judge = judge;
            ViceJudge1 = viceJudge1;
            ViceJudge2 = viceJudge2;
            Topic = topic;

            // Sorted by role (Defense, Plaintiff, Juror), then by last name
            var viewAvailable = (ListCollectionView)CollectionViewSource.GetDefaultView(AvailableSpeakers);
            viewAvailable.CustomSort = new ICJMemberComparer();

            _ = LoadWarningsAsync();
        }

        private async Task LoadWarningsAsync()
        {
            try
            {
                var warnedMembers = await _dataAccess.LoadICJMembersWithWarningsAsync();

                foreach (var warnedMember in warnedMembers)
                {
                    // Use the same instance as the available list, so counts stay in sync
                    var existingMember = AvailableSpeakers.FirstOrDefault(m =>
                        m.Member.Name == warnedMember.Name && m.Member.Kind == warnedMember.Kind);

                    if (existingMember != null)
                    {
                        existingMember.Warnings = warnedMember.Warnings;
                        WarnedList.Add(existingMember);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR loading ICJ warnings: {ex.Message}");
            }
        }

        protected override async Task SaveWarningsAsync(ICJRollCallMember member)
        {
            if (!string.IsNullOrEmpty(member.Member.Name) && !string.IsNullOrEmpty(member.Member.Kind))
            {
                await _dataAccess.UpdateICJMemberWarningsAsync(member.Member.Name, member.Member.Kind, member.Warnings);
            }
        }

        /// <summary>Opens voting: only the present jurors vote in the ICJ.</summary>
        [RelayCommand]
        private void OpenVoting()
        {
            var voters = _presentMembers
                .Where(m => m.Member.Kind != null && m.Member.Kind.Equals("juror", StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (voters.Count == 0)
            {
                AppDialog.Show(
                    Properties.Settings.Default.Lang switch
                    {
                        "fr" => "Aucun juré disponible pour voter.",
                        "es" => "No hay jurados disponibles para votar.",
                        _ => "No jurors available to vote."
                    },
                    Properties.Settings.Default.Lang switch
                    {
                        "fr" => "Aucun juré",
                        "es" => "Sin jurados",
                        _ => "No Jurors"
                    },
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            PauseClock();
            _navigateToVoting(voters);
        }
    }
}
