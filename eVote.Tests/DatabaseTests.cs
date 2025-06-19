using Xunit;
using Xunit;
using Microsoft.EntityFrameworkCore;
using eVote.src.Models;
using eVote.src.Repository;

public class DatabaseTests
{
    public DbContextOptions<EVoteDbContext> Options { get; }
    public DatabaseTests()
    {
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
    public void CanConnectAndQueryDatabase()
    {
        // Ensure database can be created and a record can be added
        using (var context = new EVoteDbContext(Options))
        {
            context.Database.EnsureCreated();
            var user = new User { Email = "healthcheck@example.com" , Password = "password"};
            context.Users.Add(user);
            context.SaveChanges();
        }

        // Ensure the record can be retrieved
        using (var context = new EVoteDbContext(Options))
        {
            var user = context.Users.FirstOrDefault(u => u.Email == "healthcheck@example.com");
            Assert.NotNull(user);
        }
    }


    [Fact]
    public void CanAddAndRetrieveUser()
    {
        // Check no users exist initially
        using (var context = new EVoteDbContext(Options))
        {
            Assert.Equal(0, context.Users.Count());
        }

        // Add a user
        using (var context = new EVoteDbContext(Options))
        {
            var user = new User { Email = "test@example.com" , Password = "1234" };
            context.Users.Add(user);
            context.SaveChanges();
        }

        // Retrieve the user
        using (var context = new EVoteDbContext(Options))
        {
            var user = context.Users.FirstOrDefault(u => u.Email == "test@example.com");
            Assert.NotNull(user);
            Assert.Equal("test@example.com", user.Email);
            Assert.Equal("1234", user.Password);
        }
    }

    [Fact]
    public void AddAndRemove10Users()
    {
        // Check no users exist initially
        using (var context = new EVoteDbContext(Options))
        {
            Assert.Equal(0, context.Users.Count());
            for (var i = 0; i < 10; i++)
            {
                var user = new User { Email = i.ToString(), Password = i.ToString() };
                context.Users.Add(user);
                context.SaveChanges();
            }
            Assert.Equal(10, context.Users.Count());
            for (var i = 0; i < 10; i++)
            {
                var user = context.Users.FirstOrDefault();
                context.Users.Remove(user);
                context.SaveChanges();
            }
            Assert.Equal(0, context.Users.Count());
        }
    }

    [Fact]
    public void AddSameEmailMultipleTimes()
    {
        using (var context = new EVoteDbContext(Options))
        {
            var user = new User { Email = "John@gmail.com", Password = "1" };
            context.Users.Add(user);
            context.SaveChanges();
            var user2 = new User { Email = "John@gmail.com", Password = "1" };
            context.Users.Add(user2);
            Assert.Throws<DbUpdateException>(() => context.SaveChanges());
            Assert.Equal(1, context.Users.Count());
            var user3 = new User { Email = "John@gmail.com", Password = "1234" };
            context.Users.Add(user3);
            Assert.Throws<DbUpdateException>(() => context.SaveChanges());
            Assert.Equal(1, context.Users.Count());

        }
    }

    [Fact]
    public void RegisterLoginTest()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => DbUser.Login("John@gmail.com", "1"));
        Assert.Equal("User does not exist", ex.Message);

        DbUser? dbUser = DbUser.RegisterUserAsync("John@gmail.com", "1")?.Result;
        Assert.NotNull(dbUser);

        Assert.Throws<InvalidOperationException>(() => DbUser.RegisterUserAsync("John@gmail.com", "1").Result);

        ex = Assert.Throws<InvalidOperationException>(() => DbUser.Login("John@gmail.com", "WrongPassword"));
        Assert.Equal("Incorrect password", ex.Message);

        dbUser = DbUser.Login("John@gmail.com", "1");
        Assert.NotNull(dbUser);
    }

    [Fact]
    public void RegisterTest()
    {
        DbUser? dbUser = DbUser.RegisterUserAsync("John@gmail.com", "1")?.Result;
        Assert.NotNull(dbUser);

        dbUser.UnregisterAsCandidate
    }
}
