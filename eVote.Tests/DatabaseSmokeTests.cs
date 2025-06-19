using Xunit;
using Xunit;
using Microsoft.EntityFrameworkCore;
using eVote.src.Models;
using eVote.src.Repository;

public class DatabaseSmokeTests
{
    [Fact]
    public void CanConnectAndQueryDatabase()
    {
        var options = new DbContextOptionsBuilder<EVoteDbContext>()
            .UseSqlite("Data Source=evote.db")
            .Options;

        // Ensure database can be created and a record can be added
        using (var context = new EVoteDbContext())
        {
            context.Database.EnsureCreated();
            var user = new User { Email = "healthcheck@example.com" };
            context.Users.Add(user);
            context.SaveChanges();
        }

        // Ensure the record can be retrieved
        using (var context = new EVoteDbContext())
        {
            var user = context.Users.FirstOrDefault(u => u.Email == "healthcheck@example.com");
            Assert.NotNull(user);
        }
    }

    [Fact]
    public void TestDatabaseConnection()
    {
        

        
    }

    [Fact]
    public void CanAddAndRetrieveUser()
    {
        var options = new DbContextOptionsBuilder<EVoteDbContext>()
            .UseSqlite("Data Source=evote.db")
            .Options;

        // Add a user
        using (var context = new EVoteDbContext())
        {
            var user = new User { Email = "test@example.com" };
            context.Users.Add(user);
            context.SaveChanges();
        }

        // Retrieve the user
        using (var context = new EVoteDbContext())
        {
            var user = context.Users.FirstOrDefault(u => u.Email == "test@example.com");
            Assert.NotNull(user);
            Assert.Equal("test@example.com", user.Email);
        }
    }
}
