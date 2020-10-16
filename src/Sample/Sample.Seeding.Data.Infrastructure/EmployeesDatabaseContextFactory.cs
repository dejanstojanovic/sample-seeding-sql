using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Sample.Seeding.Data.Infrastructure.Constants;
using System.IO;

namespace Sample.Seeding.Data.Infrastructure
{
    public class EmployeesDatabaseContextFactory : IDesignTimeDbContextFactory<EmployeesDatabaseContext>
    {

        public EmployeesDatabaseContext CreateDbContext(string[] args)
        {
            var dbContext = new EmployeesDatabaseContext(new DbContextOptionsBuilder<EmployeesDatabaseContext>().UseSqlServer(
                 new ConfigurationBuilder()
                     .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), $"appsettings.json"))
                     .AddEnvironmentVariables()
                     .Build()
                     .GetConnectionString(DbContextConfigConstants.DB_CONNECTION_CONFIG_NAME)
                 ).Options);
            dbContext.Database.Migrate();
            return dbContext;
        }
    }
}
