using Foscamun2026.Properties;

public class Country
{
    public string IsoCode { get; set; } = "";
    public string EnglishName { get; set; } = "";
    public string FrenchName { get; set; } = "";
    public string SpanishName { get; set; } = "";

    public int Warnings { get; set; }

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

    //public string SmallFlagPath => $"/Resources/24/{IsoCode}.png";
    //public string LargeFlagPath => $"/Resources/large/{IsoCode}.png";
    public string FlagPath => $"pack://application:,,,/Resources/flags/{IsoCode.ToLower()}.svg";

    // Override Equals per confrontare per IsoCode
    public override bool Equals(object? obj)
    {
        if (obj is Country other)
        {
            return IsoCode == other.IsoCode;
        }
        return false;
    }

    // Override GetHashCode per coerenza con Equals
    public override int GetHashCode()
    {
        return IsoCode?.GetHashCode() ?? 0;
    }
}