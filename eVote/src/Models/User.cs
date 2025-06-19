global using UserId = System.Guid;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eVote.src.Models
{
    public class User
    {
        // Primary key (ensures a user is associated with an email)
        [Key]
        public UserId Id = new UserId();
        [Required]
        public string Email { get; set; } = string.Empty; // Also acts as a unique identifier

        public string Password { get; set; } = string.Empty;
        public bool IsCandidate { get; set; } = false;
    }

    public class Vote
    {
        public UserId VoterId { get; set; }
        public UserId CandidateId { get; set; }
    }
}