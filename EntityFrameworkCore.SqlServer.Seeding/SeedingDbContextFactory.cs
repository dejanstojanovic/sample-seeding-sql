using EntityFrameworkCore.SqlServer.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Sample.Seeding.Data.Infrastructure
{
    public class SeedingDbContextFactory : IDesignTimeDbContextFactory<SeedingDbContext>
    {

        public SeedingDbContext CreateDbContext(string[] args)
        {
            var dbContext = new SeedingDbContext(new DbContextOptionsBuilder<SeedingDbContext>().UseSqlServer(
                 new ConfigurationBuilder()
                     .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), $"appsettings.json"))
                     .Build()
                     .GetConnectionString("SeedingDatabase")
                 ).Options);
            dbContext.Database.Migrate();
            return dbContext;
        }
    }
}
