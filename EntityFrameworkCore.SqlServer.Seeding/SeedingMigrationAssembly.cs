using EntityFrameworkCore.SqlServer.Seeding.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace EntityFrameworkCore.SqlServer.Seeding
{
    public class SeedingMigrationAssembly : IMigrationsAssembly
    {
        readonly Type _contextType;
      
        public SeedingMigrationAssembly(ICurrentDbContext currentContext)
        {
            _contextType = currentContext.Context.GetType();

        }
        public IReadOnlyDictionary<string, TypeInfo> Migrations 
            => new Dictionary<String, TypeInfo>() {
                { typeof(AddSeedingTrackingMigration).Name, typeof(AddSeedingTrackingMigration).GetTypeInfo() }
                };

        public ModelSnapshot ModelSnapshot => throw new NotImplementedException();

        public Assembly Assembly => typeof(void).Assembly;

        public Migration CreateMigration(TypeInfo migrationClass, string activeProvider)
        {
            var migration = (Migration)Activator.CreateInstance(migrationClass.AsType());
            migration.ActiveProvider = activeProvider;

            return migration;
        }

        public string FindMigrationId(string nameOrId)
        {
            throw new NotImplementedException();
        }
    }
}
