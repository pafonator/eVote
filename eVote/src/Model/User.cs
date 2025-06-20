global using UserId = System.Guid;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace eVote.src.Model
{
    public class User
    {
        // Primary key (ensures a user is associated with an email)
        [Key]
        public UserId Id { get; set; } = new UserId();
        [Required]
        [EmailAddress]
        public required string Email { get; set; } // Also acts as a unique identifier
        [Required]
        public required string Password { get; set; }
        public bool IsCandidate { get; set; } = false;
    }

    [PrimaryKey(nameof(VoterId), nameof(CandidateId))]
    public class Vote
    {
        public UserId VoterId { get; set; }
        public UserId CandidateId { get; set; }
    }
}