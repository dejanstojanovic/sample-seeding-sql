using EntityFrameworkCore.SqlServer.Seeding.Configurations;
using EntityFrameworkCore.SqlServer.Seeding.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.SqlServer.Seeding
{
    public class SeedingDbContext:DbContext
    {
        public SeedingDbContext() : base()
        {

        }

        public SeedingDbContext(DbContextOptions<SeedingDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new SeedingEntryConfiguration());
            
        }
        internal virtual DbSet<SeedingEntry> SeedingEntries { get; set; }
    }
}
