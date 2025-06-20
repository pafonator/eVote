using eVote.src.Repository;
using Microsoft.EntityFrameworkCore;

namespace eVote.src.Model.DTO
{
    public class TableRow
    {
        public UserId Id { get; set; }
        public string Email { get; set; } = "";
        public bool IsCandidate { get; set; }
        public int VoteCount { get; set; }
    };

    public class UserCredentials
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
