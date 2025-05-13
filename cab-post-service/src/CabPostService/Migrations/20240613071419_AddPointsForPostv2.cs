using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CabPostService.Migrations
{
    public partial class AddPointsForPostv2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOutstanding",
                table: "Posts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTopLegendary",
                table: "Posts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTopTrending",
                table: "Posts",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOutstanding",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "IsTopLegendary",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "IsTopTrending",
                table: "Posts");
        }
    }
}
