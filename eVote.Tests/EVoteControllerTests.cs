using System.Security.Claims;
using System.Threading.Tasks;
using eVote.src.Controller;
using eVote.src.Model;
using eVote.src.Model.DTO;
using eVote.src.Repository;
using eVote.src.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Xunit;

namespace eVote.Tests;

public class EVoteControllerTests
{
    public DbContextOptions<EVoteDbContext> Options { get; }
    private readonly JwtToken _jwtServiceMock; //TODO use an actual mock
    private readonly EVoteController _controller;

    public EVoteControllerTests()
    {
        _jwtServiceMock = new JwtToken("SECRET_KEY");
        _controller = new EVoteController(_jwtServiceMock);

        // Ensure an empty database is created before each test
        Options = new DbContextOptionsBuilder<EVoteDbContext>()
            .UseSqlite("Data Source=evoteTests.db")
            .Options;

        EVoteDbContext.DbPath = "evoteTests.db"; // Path to the SQLite database file for tests

        using (var context = new EVoteDbContext(Options))
        {
            context.Database.EnsureDeleted(); // Delete existing database
            context.Database.Migrate(); // Create a new empty database
        }
    }

    [Fact]
    public async Task GetUsersWithVotesAsync_ReturnsOk()
    {
        // Arrange
        // Mock DbRead.GetUsersWithVotesAsync if possible

        // Act
        var result = await _controller.GetUsersWithVotesAsync();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetRemainingVotesAsync_ReturnsOk()
    {
        var result = await _controller.GetRemainingVotesAsync();
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetCurrentUser_ReturnsOk_WhenAuthorized()
    {
        // Arrange
        var userId = "test-user-id";
        var claims = new[] { new Claim("Id", userId) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        // Act
        var result = await _controller.GetCurrentUser();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetCurrentUserVotes_ReturnsOk_WhenAuthorized()
    {
        var userId = "test-user-id";
        var claims = new[] { new Claim("Id", userId) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        var result = await _controller.GetCurrentUserVotes();
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Register_ReturnsOk_WithToken()
    {
        var credentials = new UserCredentials { Email = "test@example.com", Password = "password" };

        var result = await _controller.Register(credentials);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task Login_ReturnsOk_WithToken()
    {
        var credentials = new UserCredentials { Email = "test@example.com", Password = "password" };

        var result = await _controller.Login(credentials);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task BecomeCandidate_ReturnsOk_WhenAuthorized()
    {
        var userId = "test-user-id";
        var claims = new[] { new Claim("Id", userId) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        var result = await _controller.BecomeCandidate();
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task UnbecomeCandidate_ReturnsOk_WhenAuthorized()
    {
        var userId = "test-user-id";
        var claims = new[] { new Claim("Id", userId) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        var result = await _controller.UnbecomeCandidate();
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task AddVote_ReturnsOk_WhenAuthorized()
    {
        var userId = "test-user-id";
        var claims = new[] { new Claim("Id", userId) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        var candidateId = new Guid(); // Use a valid UserId instance as needed
        var result = await _controller.AddVote(candidateId);
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task RemoveVote_ReturnsOk_WhenAuthorized()
    {
        var userId = "test-user-id";
        var claims = new[] { new Claim("Id", userId) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        var candidateId = new Guid(); // Use a valid UserId instance as needed
        var result = await _controller.RemoveVote(candidateId);
        Assert.IsType<OkResult>(result);
    }
}
