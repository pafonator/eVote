using eVote.src.DTO;
using eVote.src.Models;
using eVote.src.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eVote.src.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class EVoteController : ControllerBase
    {
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("Hello from the API");
        }

        [HttpGet]
        public async Task<List<UserWithVotesDto>> GetUsersWithVotesAsync()
        {
            await using var db = EVoteDbContext.GetDb();

            var voteCounts = await db.Votes
                .GroupBy(v => v.CandidateId)
                .Select(g => new { CandidateId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.CandidateId, g => g.Count);

            return await db.Users
                .Select(u => new UserWithVotesDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    IsCandidate = u.IsCandidate,
                    VoteCount = voteCounts.ContainsKey(u.Id) ? voteCounts[u.Id] : 0
                })
                .ToListAsync();
        }



        [HttpPost("vote")]
        public async Task<IActionResult> VoteAsync(UserId userId)
        {
            //TODO
            return Ok(userId);
        }
    }
}
