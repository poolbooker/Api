using Microsoft.EntityFrameworkCore;
using Pb.Api.Entities;

namespace Pb.Api.Helpers
{
    public class PoolBookerDbContext : DbContext
    {
        public DbSet<Country> Countries { get; set; }

        public DbSet<Account> Accounts { get; set; }

        //public DbSet<Announcement> Announcements { get; set; }

        public PoolBookerDbContext(DbContextOptions<PoolBookerDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Country>().ToTable("Country");
            modelBuilder.Entity<Account>().ToTable("Account");
            //modelBuilder.Entity<Announcement>().ToTable("Announcement");
        }
    }
}