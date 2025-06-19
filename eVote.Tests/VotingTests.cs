using Xunit;
using Microsoft.EntityFrameworkCore;
using eVote.src.Repository;
using eVote.src.Models;


public class VotingTests
{
    [Fact]
    public void UserCannotVoteForSelf()
    {
        var options = new DbContextOptionsBuilder<EVoteDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        using var context = new EVoteDbContext(options);
        var user = new User { Email = "user1@example.com" };
        context.Users.Add(user);
        context.SaveChanges();

        // Simulate voting logic here and assert the rule
        // Example: Assert.Throws<InvalidOperationException>(() => ...);
    }
}
