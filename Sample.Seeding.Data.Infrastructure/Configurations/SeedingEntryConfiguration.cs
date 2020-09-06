using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sample.Seeding.Data.Infrastructure.Entities;

namespace Sample.Seeding.Data.Infrastructure.Configurations
{
    class SeedingEntryConfiguration : IEntityTypeConfiguration<SeedingEntry>
    {
        public void Configure(EntityTypeBuilder<SeedingEntry> builder)
        {
            builder.ToTable("__SeedingHistory");
            builder.HasKey(s => s.Name);
        }
    }
}
