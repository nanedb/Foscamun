using System.Windows;

namespace Foscamun2026
{
    public partial class StartWindow : Window
    {
        public StartWindow()
        {
            InitializeComponent();
        }

        private void RollCall_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Roll Call starting...");
        }

        private void Voting_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Voting session starting...");
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
