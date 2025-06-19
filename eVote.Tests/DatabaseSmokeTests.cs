using Xunit;
using Microsoft.EntityFrameworkCore;
using eVote.src.Models;
using eVote.src.Repository;

public class DatabaseSmokeTests
{
    [Fact]
    public void CanAddAndRetrieveUser()
    {
        var options = new DbContextOptionsBuilder<EVoteDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_CanAddAndRetrieveUser")
            .Options;

        // Add a user
        using (var context = new EVoteDbContext(options))
        {
            var user = new User { Email = "test@example.com" };
            context.Users.Add(user);
            context.SaveChanges();
        }

        // Retrieve the user
        using (var context = new EVoteDbContext(options))
        {
            var user = context.Users.FirstOrDefault(u => u.Email == "test@example.com");
            Assert.NotNull(user);
            Assert.Equal("test@example.com", user.Email);
        }
    }
}
