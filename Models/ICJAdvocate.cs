using System.Text.Json.Serialization;

namespace Foscamun.Models
{
    /// <summary>
    /// An ICJ advocate (plaintiff or defense) and the country they represent.
    /// </summary>
    public class ICJAdvocate
    {
        public string Name { get; set; } = "";

        public string IsoCode { get; set; } = "";

        /// <summary>
        /// Gets the pack URI path to the SVG flag of the represented country.
        /// </summary>
        [JsonIgnore]
        public string FlagPath => $"pack://application:,,,/Resources/flags/{IsoCode.ToLower()}.svg";
    }
}
