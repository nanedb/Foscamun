namespace Foscamun.Models
{
    /// <summary>
    /// Someone who can take the floor in a session: a country in a committee,
    /// an advocate or a juror in the ICJ. The session page binds to these members.
    /// </summary>
    public interface ISpeaker
    {
        /// <summary>Name shown in the lists and in the "now speaking" panel.</summary>
        string Name { get; }

        /// <summary>Pack URI of the flag, or null when there is none (ICJ jurors).</summary>
        string? FlagPath { get; }

        /// <summary>Warnings received so far.</summary>
        int Warnings { get; set; }
    }
}
