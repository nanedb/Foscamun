using Foscamun.Models;

/// <summary>
/// Represents the International Court of Justice (ICJ) configuration.
/// Includes judges, advocates (plaintiff/defense), jurors, and case topic.
/// </summary>
public class ICJ
{
    public string Judge { get; set; } = "";
    public string ViceJudge1 { get; set; } = "";
    public string ViceJudge2 { get; set; } = "";
    public string Topic { get; set; } = "";

    /// <summary>
    /// Plaintiff advocates, each with the country they represent.
    /// </summary>
    public List<ICJAdvocate> Plaintiffs { get; set; } = new();

    /// <summary>
    /// Defense advocates, each with the country they represent.
    /// </summary>
    public List<ICJAdvocate> Defense { get; set; } = new();

    public List<string> Jurors { get; set; } = new();

    /// <summary>
    /// Warnings for each plaintiff advocate (3 advocates).
    /// </summary>
    public List<int> PlaintiffWarnings { get; set; } = new() { 0, 0, 0 };

    /// <summary>
    /// Warnings for each defense advocate (3 advocates).
    /// </summary>
    public List<int> DefenseWarnings { get; set; } = new() { 0, 0, 0 };

    /// <summary>
    /// Warnings for each juror.
    /// </summary>
    public List<int> JurorsWarnings { get; set; } = new();
}
