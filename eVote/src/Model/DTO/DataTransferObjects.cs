using eVote.src.Repository;
using Microsoft.EntityFrameworkCore;

namespace eVote.src.Model.DTO
{

    public class UserVoteInfo
    {
        public UserId Id { get; set; }
        public string Email { get; set; } = "";
        public bool IsCandidate { get; set; }
        public int VoteCount { get; set; }
    };

    public class UserInfo
    {
        public UserId Id { get; set; }
        public string Email { get; set; } = "";
        public bool IsCandidate { get; set; }
    };

    public class UserCredentials
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class TokenAuthentication
    {
        public string Token { get; set; } // The JWT itself
    };
}
