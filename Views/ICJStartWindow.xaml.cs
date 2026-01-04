using System.Windows;

namespace Foscamun2026
{
    public partial class ICJStartWindow : Window
    {
        public ICJStartWindow()
        {
            InitializeComponent();
        }

        private void StartCase_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("ICJ Case starting...");
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
