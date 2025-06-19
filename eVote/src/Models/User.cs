namespace eVote.src.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Email { get; set; } = string.Empty;
        public bool IsCandidate { get; set; }
    }

    public class Voter : User
    {
        public Guid userId { get; set; }
        public Guid voteFor { get; set; }
    }

    public class Candidate : User
    {
        public uint voteCount { get; set; } = 0;

    }
}
