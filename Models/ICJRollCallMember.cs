using CommunityToolkit.Mvvm.ComponentModel;

namespace Foscamun2026.Models
{
    /// <summary>
    /// Wrapper class for ICJMember to display in Roll Call with formatted names
    /// </summary>
    public partial class ICJRollCallMember : ObservableObject
    {
        public ICJMember Member { get; set; }

        [ObservableProperty]
        private int warnings;

        public string? IsoCode => Member.IsoCode;
        
        public string? FlagPath => string.IsNullOrEmpty(IsoCode) 
            ? null 
            : $"pack://application:,,,/Resources/flags/{IsoCode.ToLower()}.svg";

        /// <summary>
        /// Display name formatted as:
        /// - "Adv. [Name] - Plaintiff" or "Adv. [Name] - Defense" for advocates
        /// - "Juror [Name]" for jurors
        /// </summary>
        public string DisplayName
        {
            get
            {
                if (Member.Kind == null)
                    return Member.Name ?? "";

                return Member.Kind.ToLower() switch
                {
                    "plaintiff" => $"Adv. {Member.Name} - Plaintiff",
                    "defense" => $"Adv. {Member.Name} - Defense",
                    "juror" => $"Juror {Member.Name}",
                    _ => Member.Name ?? ""
                };
            }
        }

        public ICJRollCallMember(ICJMember member)
        {
            Member = member;
            Warnings = member.Warnings;
        }
    }
}
