using EntityFrameworkCore.SqlServer.Seeding.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EntityFrameworkCore.SqlServer.Seeding.Extensions
{
    /// <summary>
    /// SQL server script seeding extensions
    /// </summary>
    public static class SeedingExtensions
    {
        /// <summary>
        /// Enables script seeding
        /// </summary>
        /// <param name="services">Services collection</param>
        /// <param name="connectionString">Database connection string</param>
        public static void AddScriptSeeding(this IServiceCollection services, String connectionString)
        {
            services.AddDbContext<SeedingDbContext>(options =>
            {
                options.UseSqlServer(connectionString,
                    x =>
                    {
                        x.MigrationsAssembly(typeof(SeedingExtensions).Assembly.GetName().Name);
                    }
                );
            });
        }

        /// <summary>
        /// Seeds scripts
        /// </summary>
        /// <param name="app">Application builder instance</param>
        /// <param name="seedingAssembly">Assembly containing scripts</param>
        /// <param name="resourceFolder">Folder inside the project were scripts are located</param>
        public static void SeedFromScripts(this IApplicationBuilder app, Assembly seedingAssembly, String resourceFolder = "Seedings")
        {
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<SeedingDbContext>())
                {
                    context.Database.Migrate();
                    var files = seedingAssembly.GetManifestResourceNames();

                    var executedSeedings = context.SeedingEntries.ToArray();
                    var folderPathSegment = !String.IsNullOrWhiteSpace(resourceFolder) ? $"{resourceFolder}." : String.Empty;
                    var filePrefix = $"{seedingAssembly.GetName().Name}.{folderPathSegment}";
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
                        using (Stream stream = seedingAssembly.GetManifestResourceStream(file.PhysicalFile))
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
                                context.SeedingEntries.Add(new SeedingEntry() { Name = file.LogicalFile });
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
