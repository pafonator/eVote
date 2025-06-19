namespace eVote.src.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Email { get; set; } = string.Empty;
        public bool IsCandidate { get; set; }

        // Navigation properties
        public Vote Vote { get; set; } = new Vote();
        public ICollection<Vote> VotesReceived { get; set; } = new List<Vote>();
    }

    public class Vote
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // Foreign keys
        public Guid VoterId { get; set; }
        public Guid CandidateId { get; set; }
    }
}