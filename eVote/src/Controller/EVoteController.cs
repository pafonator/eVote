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

        [HttpGet("table")]
        public async Task<IActionResult> GetUsersWithVotesAsync()
        {
            var table = await DbRead.GetUsersWithVotesAsync();
            return Ok(table);
        }

        [HttpGet("remainingVotes")]
        public async Task<IActionResult> GetRemainingVotesAsync()
        {
            var votes = await DbRead.GetRemainingVotes();
            return Ok(votes);
        }

        [HttpGet("user/info")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            UserId userId = UserId.Parse(User.FindFirst("Id")?.Value);
            var user = await DbRead.GetUserAsync(userId);
            return Ok(new UserInfo
            {
                Email = user?.Email ?? "",
                Id = user?.Id ?? UserId.Empty,
                IsCandidate = user?.IsCandidate ?? false
            });
        }

        [HttpGet("user/votes")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUserVotes()
        {
            UserId userId = UserId.Parse(User.FindFirst("Id")?.Value);
            var votes = await DbRead.GetVotesOfUserAsync(userId);
            return Ok(votes);
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

        [HttpPost("user/becomeCandidate")]
        [Authorize]
        public async Task<IActionResult> BecomeCandidate()
        {
            try
            {
                UserId userId = UserId.Parse(User.FindFirst("Id")?.Value);
                await DbUserActions.RegisterAsCandidate(userId);
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost("user/unbecomeCandidate")]
        [Authorize]
        public async Task<IActionResult> UnbecomeCandidate()
        {
            try
            {
                UserId userId = UserId.Parse(User.FindFirst("Id")?.Value);
                await DbUserActions.UnregisterAsCandidate(userId);
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost("user/addVote")]
        [Authorize]
        public async Task<IActionResult> AddVote([FromBody] UserId candidateId)
        {
            try
            {
                UserId userId = UserId.Parse(User.FindFirst("Id")?.Value);
                await DbUserActions.AddVote(userId, candidateId);
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost("user/removeVote")]
        [Authorize]
        public async Task<IActionResult> RemoveVote([FromBody] UserId candidateId)
        {
            try
            {
                UserId userId = UserId.Parse(User.FindFirst("Id")?.Value);
                await DbUserActions.RemoveVote(userId, candidateId);
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

    }
}
