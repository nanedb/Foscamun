using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Foscamun.Views
{
    /// <summary>
    /// The tally of one voting round, kept by the voting pages for the final result.
    /// </summary>
    /// <param name="CanAbstain">False for rounds without abstaining (the tally shows two numbers).</param>
    /// <param name="OutcomeKey">String key of why the vote went on (e.g. "NoUnanimityLbl"); ignored for the decisive round.</param>
    public record RoundTally(int InFavor, int Abstained, int Against, bool CanAbstain, string OutcomeKey);

    /// <summary>One row of the rounds table.</summary>
    public record FinalRoundRow(string Label, Brush Bar, string Tally, string Outcome, Brush OutcomeBrush, bool IsFinal);

    /// <summary>
    /// Body of the committee and ICJ final result pages: outcome, every round, who voted what
    /// in the decisive round, and buttons for the session and Home. The voting rules stay in the pages.
    /// </summary>
    public partial class FinalResultPanel : UserControl
    {
        public event EventHandler? BackRequested;

        public FinalResultPanel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Shows the outcome of the vote.
        /// </summary>
        /// <param name="context">Committee and session (e.g. "Security Council · Session 2"), or null.</param>
        /// <param name="headline">The outcome (e.g. "Resolution adopted").</param>
        /// <param name="adopted">Colours the decisive round green (adopted) or red (rejected).</param>
        /// <param name="rounds">Every round, the decisive one last.</param>
        public void Show(string? context, string headline, bool adopted,
                         IReadOnlyList<RoundTally> rounds,
                         IReadOnlyList<ResultVoter> inFavor,
                         IReadOnlyList<ResultVoter> abstained,
                         IReadOnlyList<ResultVoter> against)
        {
            var last = rounds[^1];
            string roundCount = rounds.Count == 1
                ? Text("OneRoundLbl")
                : string.Format(Text("RoundsCountFmt"), rounds.Count);
            ContextText.Text = (context == null ? roundCount : $"{context} · {roundCount}")
                .ToUpper(CultureInfo.CurrentCulture);

            HeadlineText.Text = headline;
            FinalVoteText.Text = string.Format(Text("FinalVoteFmt"),
                VoteGroups.Tally(last.InFavor, last.Abstained, last.Against, last.CanAbstain));

            var outcomeBrush = (Brush)FindResource(adopted ? "InFavorBrush" : "AgainstBrush");
            var secondary = (Brush)FindResource("TextSecondaryBrush");
            RoundList.ItemsSource = rounds.Select((r, i) =>
            {
                bool isFinal = i == rounds.Count - 1;
                return new FinalRoundRow(
                    string.Format(Text("RoundNumFmt"), i + 1),
                    VoteGroups.BarBrush(r.InFavor, r.CanAbstain ? r.Abstained : 0, r.Against),
                    VoteGroups.Tally(r.InFavor, r.Abstained, r.Against, r.CanAbstain),
                    isFinal ? Text(adopted ? "AdoptedLbl" : "RejectedLbl") : Text(r.OutcomeKey),
                    isFinal ? outcomeBrush : secondary,
                    isFinal);
            }).ToList();

            Groups.Show(inFavor, abstained, against, last.CanAbstain);
        }

        private string Text(string key) => (string)FindResource(key);

        private void Back_Click(object sender, RoutedEventArgs e) => BackRequested?.Invoke(this, EventArgs.Empty);

        private void Home_Click(object sender, RoutedEventArgs e) => MainWindow.Instance.GoHomeCommand.Execute(null);
    }
}
