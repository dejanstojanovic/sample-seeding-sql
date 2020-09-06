﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sample.Seeding.Data.Infrastructure;

namespace Sample.Seeding.Data.Infrastructure.Migrations
{
    [DbContext(typeof(EmployeesDatabaseContext))]
    [Migration("20200906175423_Add_Seeding_Tracking")]
    partial class Add_Seeding_Tracking
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Sample.Seeding.Data.Infrastructure.Entities.SeedingEntry", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Name");

                    b.ToTable("__SeedingHistory");
                });
#pragma warning restore 612, 618
        }
    }
}