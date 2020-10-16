using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sample.Seeding.Data.Infrastructure.Constants;
using System;
using System.IO;
using System.Linq;

namespace Sample.Seeding.Data.Infrastructure.Extensions
{
    public static class DbContextExtensions
    {
        public static void AddEmployeesDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<EmployeesDatabaseContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString(DbContextConfigConstants.DB_CONNECTION_CONFIG_NAME),
                    x =>
                    {
                        x.MigrationsHistoryTable("__EFMigrationsHistory");
                        x.MigrationsAssembly(typeof(DbContextExtensions).Assembly.GetName().Name);
                    }
                );
            });
        }

        public static void MigrateEmployeesData(this IApplicationBuilder app, IConfiguration configuration)
        {
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<EmployeesDatabaseContext>())
                {
                    context.Database.Migrate();
                }
            }
        }
    }
}
