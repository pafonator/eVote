using Microsoft.EntityFrameworkCore;
using eVote.src.Models;

namespace eVote.src.Repository
{
    public class EVoteDbContext : DbContext
    {
        public EVoteDbContext(DbContextOptions<EVoteDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        // Add other DbSets as needed, e.g. Votes

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure entity relationships and constraints here if needed
        }
    }
}
