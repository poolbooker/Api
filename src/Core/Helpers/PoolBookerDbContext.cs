using Microsoft.EntityFrameworkCore;
using Pb.Api.Entities;

namespace Pb.Api.Helpers
{
    public class PoolBookerDbContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        
        public PoolBookerDbContext(DbContextOptions<PoolBookerDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().ToTable("Account");
        }
    }
}