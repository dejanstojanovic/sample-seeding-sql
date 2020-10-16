using Microsoft.EntityFrameworkCore.Migrations;

namespace EntityFrameworkCore.SqlServer.Seeding.Migrations
{
    public partial class Add_Seeding_Tracking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "__SeedingHistory",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK___SeedingHistory", x => x.Name);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "__SeedingHistory");
        }
    }
}
