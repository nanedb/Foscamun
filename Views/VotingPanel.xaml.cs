using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Foscamun.Views
{
    /// <summary>
    /// Body of the committee and ICJ voting pages: shows one voter at a time and
    /// raises an event for each vote (buttons or keys A / S / D).
    /// The voting rules stay in the pages.
    /// </summary>
    public partial class VotingPanel : UserControl
    {
        private const int MaxSegmentWidth = 26;
        private const int SegmentGap = 6;
        private const double SegmentsMaxWidth = 380;
        private const double NameMaxWidth = 440;          // beside the flag
        private const double NameMaxWidthNoFlag = 660;    // ICJ jurors: the name has the whole width

        private Window? _window;

        public event EventHandler? Approved;
        public event EventHandler? Abstained;
        public event EventHandler? Rejected;

        public VotingPanel()
        {
            InitializeComponent();
            Loaded += (_, _) =>
            {
                _window = Window.GetWindow(this);
                if (_window != null) _window.PreviewKeyDown += Window_PreviewKeyDown;
            };
            Unloaded += (_, _) =>
            {
                if (_window != null) _window.PreviewKeyDown -= Window_PreviewKeyDown;
                _window = null;
            };
        }

        /// <summary>Abstaining is not allowed in some rounds: the button disappears.</summary>
        public bool CanAbstain
        {
            get => AbstainButton.Visibility == Visibility.Visible;
            set => AbstainButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>Committee (or court) and session, shown top right.</summary>
        public void SetContext(string text) => ContextText.Text = text;

        /// <summary>
        /// Shows the voter whose turn it is.
        /// </summary>
        /// <param name="round">Voting round (1-3).</param>
        /// <param name="voted">How many have voted already.</param>
        /// <param name="total">How many vote in all.</param>
        /// <param name="flagPath">Flag pack URI, or null (ICJ jurors).</param>
        /// <param name="name">Name of the voter.</param>
        /// <param name="next">Names of the following voters (the first three are shown).</param>
        public void ShowVoter(int round, int voted, int total, string? flagPath, string name, IEnumerable<string> next)
        {
            ProgressText.Text = string.Format((string)FindResource("VotingProgressFmt"), round, voted, total);

            if (string.IsNullOrEmpty(flagPath))
            {
                FlagView.Visibility = Visibility.Collapsed;
                VoterText.MaxWidth = NameMaxWidthNoFlag;
            }
            else
            {
                FlagView.Source = new Uri(flagPath);
                FlagView.Visibility = Visibility.Visible;
                VoterText.MaxWidth = NameMaxWidth;
            }
            NameText.Text = name;

            var following = next.Take(3).ToList();
            NextText.Text = following.Count > 0
                ? $"{FindResource("NextVotersLbl")} {string.Join(" · ", following)}"
                : string.Empty;

            DrawSegments(voted, total);
        }

        // One segment per voter, white once voted; narrower when there are many voters
        private void DrawSegments(int voted, int total)
        {
            Segments.Items.Clear();
            if (total <= 0) return;

            double width = Math.Min(MaxSegmentWidth, (SegmentsMaxWidth + SegmentGap) / total - SegmentGap);
            width = Math.Max(3, width);
            var done = (Brush)FindResource("WhiteBrush");
            var todo = (Brush)FindResource("SegmentTodoBrush");

            for (int i = 0; i < total; i++)
            {
                Segments.Items.Add(new Rectangle
                {
                    Width = width,
                    Height = 4,
                    RadiusX = 2,
                    RadiusY = 2,
                    Margin = new Thickness(0, 0, i < total - 1 ? SegmentGap : 0, 0),
                    Fill = i < voted ? done : todo
                });
            }
        }

        private void Approve_Click(object sender, RoutedEventArgs e) => Approved?.Invoke(this, EventArgs.Empty);
        private void Abstain_Click(object sender, RoutedEventArgs e) => Abstained?.Invoke(this, EventArgs.Empty);
        private void Reject_Click(object sender, RoutedEventArgs e) => Rejected?.Invoke(this, EventArgs.Empty);

        // Keyboard shortcuts; a held key must not cast several votes
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.IsRepeat || Keyboard.Modifiers != ModifierKeys.None) return;
            switch (e.Key)
            {
                case Key.A:
                    e.Handled = true;
                    Approved?.Invoke(this, EventArgs.Empty);
                    break;
                case Key.S when CanAbstain:
                    e.Handled = true;
                    Abstained?.Invoke(this, EventArgs.Empty);
                    break;
                case Key.D:
                    e.Handled = true;
                    Rejected?.Invoke(this, EventArgs.Empty);
                    break;
            }
        }
    }
}
