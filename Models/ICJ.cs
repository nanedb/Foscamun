public class ICJ
{
    // BOARD
    public string Judge { get; set; } = "";
    public string ViceJudge1 { get; set; } = "";
    public string ViceJudge2 { get; set; } = "";
    public string Topic { get; set; } = "";

    // ADVOCATES - PLAINTIFFS
    public string Plaintiff1 { get; set; } = "";
    public string Plaintiff2 { get; set; } = "";
    public string Plaintiff3 { get; set; } = "";
    public Country PCountry { get; set; } = new();

    // ADVOCATES - DEFENSE
    public string Defense1 { get; set; } = "";
    public string Defense2 { get; set; } = "";
    public string Defense3 { get; set; } = "";
    public Country DCountry { get; set; } = new();

    // JURORS
    public List<string> Jurors { get; set; } = new();
}