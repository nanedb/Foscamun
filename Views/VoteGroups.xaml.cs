using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Foscamun.Views
{
    /// <summary>A voter as shown in a result chip: name and flag (null for ICJ jurors).</summary>
    public record ResultVoter(string Name, string? FlagPath);

    /// <summary>
    /// Who voted what: three groups of chips with their counts. The Abstained group
    /// disappears for rounds without abstaining.
    /// </summary>
    public partial class VoteGroups : UserControl
    {
        public VoteGroups()
        {
            InitializeComponent();
        }

        public void Show(IReadOnlyList<ResultVoter> inFavor,
                         IReadOnlyList<ResultVoter> abstained,
                         IReadOnlyList<ResultVoter> against,
                         bool canAbstain = true)
        {
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
        }

        /// <summary>"8–2–1", or "8–1" for a round without abstaining.</summary>
        public static string Tally(int inFavor, int abstained, int against, bool canAbstain) =>
            canAbstain ? $"{inFavor}–{abstained}–{against}" : $"{inFavor}–{against}";

        /// <summary>
        /// One rounded bar in three parts, green in favor, orange abstained, red against:
        /// a gradient with hard stops keeps the rounded ends.
        /// </summary>
        public static Brush BarBrush(int inFavor, int abstained, int against)
        {
            int total = inFavor + abstained + against;
            if (total == 0)
                return new SolidColorBrush(Color.FromArgb(0x59, 0xFF, 0xFF, 0xFF));

            var app = Application.Current;
            var green = ((SolidColorBrush)app.FindResource("InFavorBrush")).Color;
            var orange = ((SolidColorBrush)app.FindResource("WarningBrush")).Color;
            var red = ((SolidColorBrush)app.FindResource("AgainstBrush")).Color;
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

        private string Upper(string key) =>
            ((string)FindResource(key)).ToUpper(CultureInfo.CurrentCulture);
    }
}
