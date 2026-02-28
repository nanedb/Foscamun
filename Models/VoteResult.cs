namespace Foscamun.Models
{
    /// <summary>
    /// Represents the result of a committee vote.
    /// </summary>
    public class VoteResult
    {
        public int Id { get; set; }
        public string CommitteeName { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
        public int VotesFor { get; set; }
        public int VotesAgainst { get; set; }
        public int Abstentions { get; set; }
    }
}
