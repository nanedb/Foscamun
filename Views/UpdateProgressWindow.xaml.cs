using System.Windows;

namespace Foscamun.Views
{
    /// <summary>Shows how far the download of a new version has got; Cancel stops it.</summary>
    public partial class UpdateProgressWindow : Window
    {
        private readonly CancellationTokenSource _cancel = new();
        private const double TrackWidth = 404; // window width minus the margins

        public CancellationToken Token => _cancel.Token;

        public UpdateProgressWindow(string version)
        {
            InitializeComponent();

            if (Application.Current.MainWindow is { IsVisible: true } main)
                Owner = main;
            else
                WindowStartupLocation = WindowStartupLocation.CenterScreen;

            TitleText.Text = AppDialog.Text("UpdateDownloadingTitle", version);
            StatusText.Text = AppDialog.Text("UpdateDownloading");
        }

        public void Report(long done, long? total)
        {
            if (total is > 0)
            {
                ProgressFill.Width = TrackWidth * done / total.Value;
                StatusText.Text = AppDialog.Text("UpdateProgressFmt", done / 1048576.0, total.Value / 1048576.0);
            }
            else
            {
                StatusText.Text = AppDialog.Text("UpdateProgressNoTotalFmt", done / 1048576.0);
            }
        }

        public void SetStatus(string text)
        {
            StatusText.Text = text;
            ProgressFill.Width = TrackWidth;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => _cancel.Cancel();
    }
}
