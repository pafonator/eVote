using Microsoft.EntityFrameworkCore;
using eVote.src.Model;
using Microsoft.Extensions.Options;

namespace eVote.src.Repository
{
    public class EVoteDbContext : DbContext
    {
        public static string DbPath = "evote.db"; // Path to the SQLite database file

        public EVoteDbContext(DbContextOptions<EVoteDbContext> options) : base(options) { }
        public static EVoteDbContext GetDb()
        {
            var options = new DbContextOptionsBuilder<EVoteDbContext>()
                .UseSqlite($"Data Source={DbPath}")
                .Options;
            return new EVoteDbContext(options);
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Vote> Votes => Set<Vote>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure entity relationships and constraints here if needed
            modelBuilder.Entity<User>()
               .HasIndex(u => u.Email) // Ensure email is indexed for quick lookups
               .IsUnique();

            modelBuilder.Entity<User>()
               .Property(u => u.Password)
               .IsRequired(); // Ensure password is required

            modelBuilder.Entity<Vote>()
                .HasIndex(v => new {v.VoterId, v.CandidateId})
                .IsUnique();

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={DbPath}");
        }
    }
}
