namespace Foscamun.Models
{
    /// <summary>
    /// Represents a country assigned to a committee with its warning count.
    /// Links committees to countries in a many-to-many relationship.
    /// </summary>
    public class CountryList
    {
        public int CommID { get; set; }

        public string? IsoCode { get; set; }

        public int Warnings { get; set; }
    }
}
