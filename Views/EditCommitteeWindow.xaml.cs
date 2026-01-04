using System.Windows;
using Foscamun2026.Data;
using Foscamun2026.Models;

namespace Foscamun2026
{
    public partial class EditCommitteeWindow : Window
    {
        private readonly SqliteDataAccess _data = new SqliteDataAccess();
        private readonly Committee _committee;

        public EditCommitteeWindow(Committee committee)
        {
            InitializeComponent();
            _committee = committee;

            // Precompila i campi
            NameBox.Text = committee.Name;
            TopicABox.Text = committee.TopicA;
            TopicBBox.Text = committee.TopicB;
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            _committee.Name = NameBox.Text;
            _committee.TopicA = TopicABox.Text;
            _committee.TopicB = TopicBBox.Text;

            await _data.UpdateCommitteeAsync(_committee);

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
