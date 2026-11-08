using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Foscamun.Views
{
    /// <summary>A voter as shown in a result chip: name and flag (null for ICJ jurors).</summary>
    public record ResultVoter(string Name, string? FlagPath);

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
            TallyText.Text = canAbstain
                ? $"{inFavor.Count}–{abstained.Count}–{against.Count}"
                : $"{inFavor.Count}–{against.Count}";

            InFavorCaption.Text = Upper("InFavorLbl");
            AbstainedCaption.Text = Upper("AbstainedLbl");
            AgainstCaption.Text = Upper("AgainstLbl");

            InFavorCount.Text = inFavor.Count.ToString();
            AbstainedCount.Text = abstained.Count.ToString();
            AgainstCount.Text = against.Count.ToString();

            InFavorList.ItemsSource = inFavor;
            AbstainedList.ItemsSource = abstained;
            AgainstList.ItemsSource = against;
            AbstainedGroup.Visibility = canAbstain ? Visibility.Visible : Visibility.Collapsed;

            TallyBar.Background = BarBrush(inFavor.Count, canAbstain ? abstained.Count : 0, against.Count);
        }

        // One rounded bar in three parts: a gradient with hard stops keeps the rounded ends
        private Brush BarBrush(int inFavor, int abstained, int against)
        {
            var faint = new SolidColorBrush(Color.FromArgb(0x59, 0xFF, 0xFF, 0xFF));
            int total = inFavor + abstained + against;
            if (total == 0) return faint;

            var green = ((SolidColorBrush)FindResource("InFavorBrush")).Color;
            var orange = ((SolidColorBrush)FindResource("WarningBrush")).Color;
            var red = ((SolidColorBrush)FindResource("AgainstBrush")).Color;
            double a = (double)inFavor / total;
            double b = a + (double)abstained / total;

            var brush = new LinearGradientBrush { StartPoint = new Point(0, 0), EndPoint = new Point(1, 0) };
            brush.GradientStops.Add(new GradientStop(green, 0));
            brush.GradientStops.Add(new GradientStop(green, a));
            brush.GradientStops.Add(new GradientStop(orange, a));
            brush.GradientStops.Add(new GradientStop(orange, b));
            brush.GradientStops.Add(new GradientStop(red, b));
            brush.GradientStops.Add(new GradientStop(red, 1));
            return brush;
        }

        private string Format(string key, int round) =>
            string.Format((string)FindResource(key), round, MaxRounds);

        private string Upper(string key) =>
            ((string)FindResource(key)).ToUpper(CultureInfo.CurrentCulture);

        private void Back_Click(object sender, RoutedEventArgs e) => BackRequested?.Invoke(this, EventArgs.Empty);
        private void NextRound_Click(object sender, RoutedEventArgs e) => NextRoundRequested?.Invoke(this, EventArgs.Empty);
    }
}
