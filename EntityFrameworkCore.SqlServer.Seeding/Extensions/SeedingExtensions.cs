using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityFrameworkCore.SqlServer.Seeding.Extensions
{
    public static class SeedingExtensions
    {
        public static void AddScriptSeeding(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<EmployeesDatabaseContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString(DbContextConfigConstants.DB_CONNECTION_CONFIG_NAME),
                    x =>
                    {
                        x.MigrationsHistoryTable("__EFMigrationsHistory");
                        x.MigrationsAssembly(typeof(void).Assembly.GetName().Name);
                    }
                );
            });
        }
    }
}
