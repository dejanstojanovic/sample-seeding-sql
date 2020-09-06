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

        public static void SeedEmployeesData(this IApplicationBuilder app, IConfiguration configuration)
        {
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<EmployeesDatabaseContext>())
                {
                    context.Database.Migrate();

                    var assembly = typeof(DbContextExtensions).Assembly;
                    var files = assembly.GetManifestResourceNames();

                    var executedSeedings = context.SeedingEntries.ToArray();
                    var filePrefix = $"{assembly.GetName().Name}.Seedings.";
                    foreach (var file in files.Where(f => f.StartsWith(filePrefix) && f.EndsWith(".sql"))
                                              .Select(f => new
                                              {
                                                  PhysicalFile = f,
                                                  LogicalFile = f.Replace(filePrefix, String.Empty)
                                              })
                                              .OrderBy(f => f.LogicalFile))
                    {
                        if (executedSeedings.Any(e => e.Name == file.LogicalFile))
                            continue;

                        string command = string.Empty;
                        using (Stream stream = assembly.GetManifestResourceStream(file.PhysicalFile))
                        {
                            using (StreamReader reader = new StreamReader(stream))
                            {
                                command = reader.ReadToEnd();
                            }
                        }

                        if (String.IsNullOrWhiteSpace(command))
                            continue;

                        using (var transaction = context.Database.BeginTransaction())
                        {
                            try
                            {
                                context.Database.ExecuteSqlRaw(command);
                                context.SeedingEntries.Add(new Entities.SeedingEntry() { Name = file.LogicalFile });
                                context.SaveChanges();
                                transaction.Commit();
                            }
                            catch 
                            {
                                transaction.Rollback();
                                throw;
                            }
                        }

                    }
                }
            }
        }
    }
}
