using System.Windows;
using System.Windows.Controls;

namespace Foscamun.Views
{
    /// <summary>
    /// Body of the committee and ICJ result pages, shown after a round that reached no decision:
    /// tally, bar, who voted what, and buttons for the session and the next round.
    /// The voting rules stay in the pages.
    /// </summary>
    public partial class ResultPanel : UserControl
    {
        private const int MaxRounds = 3;

        public event EventHandler? BackRequested;
        public event EventHandler? NextRoundRequested;

        public ResultPanel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Shows the outcome of a round.
        /// </summary>
        /// <param name="round">Voting round (1-3).</param>
        /// <param name="headline">Why the vote goes on (e.g. "No unanimity").</param>
        /// <param name="canAbstain">False for rounds without abstaining: the Abstained group disappears.</param>
        public void Show(int round, string headline,
                         IReadOnlyList<ResultVoter> inFavor,
                         IReadOnlyList<ResultVoter> abstained,
                         IReadOnlyList<ResultVoter> against,
                         bool canAbstain = true)
        {
            RoundText.Text = Format("ResultRoundFmt", round);
            RoundOfText.Text = Format("RoundOfFmt", round);
            HeadlineText.Text = headline;
            TallyText.Text = VoteGroups.Tally(inFavor.Count, abstained.Count, against.Count, canAbstain);
            TallyBar.Background = VoteGroups.BarBrush(inFavor.Count, canAbstain ? abstained.Count : 0, against.Count);
            Groups.Show(inFavor, abstained, against, canAbstain);
        }

        private string Format(string key, int round) =>
            string.Format((string)FindResource(key), round, MaxRounds);

        private void Back_Click(object sender, RoutedEventArgs e) => BackRequested?.Invoke(this, EventArgs.Empty);
        private void NextRound_Click(object sender, RoutedEventArgs e) => NextRoundRequested?.Invoke(this, EventArgs.Empty);
    }
}
