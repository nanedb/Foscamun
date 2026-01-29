using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Foscamun2026.Models
{
    public class Committee : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private string _name = "";
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        private string _topicA = "";
        public string TopicA
        {
            get => _topicA;
            set { _topicA = value; OnPropertyChanged(); }
        }

        private string _topicB = "";
        public string TopicB
        {
            get => _topicB;
            set { _topicB = value; OnPropertyChanged(); }
        }

        private string _president = "";
        public string President
        {
            get => _president;
            set { _president = value; OnPropertyChanged(); }
        }

        private string _vicePresident = "";
        public string VicePresident
        {
            get => _vicePresident;
            set { _vicePresident = value; OnPropertyChanged(); }
        }

        private string _moderator = "";
        public string Moderator
        {
            get => _moderator;
            set { _moderator = value; OnPropertyChanged(); }
        }

        public int CommID { get; set; }
    }
}
