using Foscamun.Properties;

/// <summary>
/// Represents a country with multilingual names and flag resources.
/// </summary>
public class Country
{
    public string IsoCode { get; set; } = "";
    public string EnglishName { get; set; } = "";
    public string FrenchName { get; set; } = "";
    public string SpanishName { get; set; } = "";

    /// <summary>
    /// Number of warnings issued to this country during a session.
    /// </summary>
    public int Warnings { get; set; }

    /// <summary>
    /// Gets the country name in the current language.
    /// </summary>
    public string Name
    {
        get
        {
            return Settings.Default.Lang switch
            {
                "en" => EnglishName,
                "fr" => FrenchName,
                "es" => SpanishName,
                _ => EnglishName
            };
        }
    }

    /// <summary>
    /// Gets the pack URI path to the country's SVG flag.
    /// </summary>
    public string FlagPath => $"pack://application:,,,/Resources/flags/{IsoCode.ToLower()}.svg";

    /// <summary>
    /// Compares countries by IsoCode.
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is Country other)
        {
            return IsoCode == other.IsoCode;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return IsoCode?.GetHashCode() ?? 0;
    }
}
