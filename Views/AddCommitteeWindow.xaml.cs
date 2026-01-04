using System.Windows;
using Foscamun2026.Data;
using Foscamun2026.Models;

namespace Foscamun2026
{
    public partial class AddCommitteeWindow : Window
    {
        private readonly SqliteDataAccess _data = new SqliteDataAccess();

        public AddCommitteeWindow()
        {
            InitializeComponent();
        }

        private async void Add_Click(object sender, RoutedEventArgs e)
        {
            var committee = new Committee
            {
                Name = NameBox.Text,
                TopicA = TopicABox.Text,
                TopicB = TopicBBox.Text,
                President = "President",
                VicePresident = "Vice President",
                Moderator = "Moderator"
            };

            await _data.AddCommitteeAsync(committee);

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
