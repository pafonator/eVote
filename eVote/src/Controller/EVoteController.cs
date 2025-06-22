using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using eVote.src.Model.DTO;
using eVote.src.Repository;
using eVote.src.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace eVote.src.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class EVoteController : ControllerBase
    {
        private readonly JwtToken _jwtService;
        public EVoteController(JwtToken jwtService)
        {
            _jwtService = jwtService;
        }


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
                var user = DbUserActions.RegisterUser(content.Email, content.Password).Result;
                if (user == null)
                {
                    return Problem("An unknown problem occured");
                }

                var token = _jwtService.GenerateToken(user.Id.ToString(), user.Email);

                return Ok(new TokenAuthentication
                {
                    Token = token,
                });
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
                var user = DbUserActions.Login(content.Email, content.Password).Result;
                if (user == null)
                {
                    return Problem("An unknown problem occured");
                }

                var token = _jwtService.GenerateToken(user.Id.ToString(), user.Email);

                return Ok(new TokenAuthentication
                {
                    Token = token,
                });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet("user/test")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            var userId = User.FindFirst("Id")?.Value;
            var email = User.FindFirst("Email")?.Value;

            Console.WriteLine($"Current User: {userId}, Email: {email}");

            return Ok();
        }

    }
}
