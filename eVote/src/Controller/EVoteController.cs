using System.Text.Json;
using eVote.src.Model.DTO;
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

        [HttpGet("table")]
        public async Task<List<TableRow>> GetUsersWithVotesAsync()
        {
            await using var db = EVoteDbContext.GetDb();

            var voteCounts = await db.Votes
                .GroupBy(v => v.CandidateId)
                .Select(g => new { CandidateId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.CandidateId, g => g.Count);

            return await db.Users
                .Select(u => new TableRow
                {
                    Id = u.Id,
                    Email = u.Email,
                    IsCandidate = u.IsCandidate,
                    VoteCount = voteCounts.ContainsKey(u.Id) ? voteCounts[u.Id] : 0
                })
                .ToListAsync();
        }


        [HttpPost("user/register")]
        public async Task<IActionResult> Register([FromBody] UserCredentials content)
        {
            try
            {
                //TODO store user
                var user = DbUserActions.RegisterUser(content.Email, content.Password);
                return Ok(user);

            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost("user/login")]
        public async Task<IActionResult> Login([FromBody] UserCredentials content)
        {
            try
            {
                //TODO store user
                var user = DbUserActions.Login(content.Email, content.Password);
                return Ok(user);

            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}
