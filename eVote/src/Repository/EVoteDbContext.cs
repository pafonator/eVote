using Microsoft.EntityFrameworkCore;
using eVote.src.Models;

namespace eVote.src.Repository
{
    public class EVoteDbContext : DbContext
    {
        public EVoteDbContext(DbContextOptions<EVoteDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Vote> Votes => Set<Vote>();

        /*
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure entity relationships and constraints here if needed
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=evote.db");
        }*/
    }
}
