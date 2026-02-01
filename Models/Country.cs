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
    public string FlagPath => $"/Resources/flags/{IsoCode}.svg";
}