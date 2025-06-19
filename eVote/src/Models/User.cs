public class UserId
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; }
}

public class User
{
    public string Email { get; set; } //Id
    public bool IsCandidate { get; set; }
    public Guid VoteGiven = new();
    public List<Guid> VotesReceived { get; set; } = new();
    public User(string email)
    {
        Email = email;
    }
}

public class Candidate : User
{
    public Candidate(string email) : base(email)
    {
        IsCandidate = true;
    }
}
