using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CabPostService.Migrations
{
    public partial class AddPointsForPost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LegendaryPoint",
                table: "Posts",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OutstandingPoint",
                table: "Posts",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrendingPoint",
                table: "Posts",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LegendaryPoint",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "OutstandingPoint",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "TrendingPoint",
                table: "Posts");
        }
    }
}
