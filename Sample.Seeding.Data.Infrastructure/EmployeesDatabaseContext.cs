using Microsoft.EntityFrameworkCore;
using Sample.Seeding.Data.Infrastructure.Configurations;
using Sample.Seeding.Data.Infrastructure.Entities;
using Sample.Seeding.Domain;

namespace Sample.Seeding.Data.Infrastructure
{
    public class EmployeesDatabaseContext : DbContext
    {
        public EmployeesDatabaseContext() : base()
        {

        }

        public EmployeesDatabaseContext(DbContextOptions<EmployeesDatabaseContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new SeedingEntryConfiguration());
            modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
        }

        internal virtual DbSet<SeedingEntry> SeedingEntries { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
    }
}
