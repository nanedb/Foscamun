namespace Foscamun2026.Models
{
    public class Committee
    {
        public int CommID { get; set; }

        public string Name { get; set; } = string.Empty;

        public string TopicA { get; set; } = string.Empty;

        public string TopicB { get; set; } = string.Empty;

        public string President { get; set; } = string.Empty;

        public string VicePresident { get; set; } = string.Empty;

        public string Moderator { get; set; } = string.Empty;
    }
}
