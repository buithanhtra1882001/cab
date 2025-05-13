using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CabUserService.Migrations
{
    public partial class AddColumnUserDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Follower",
                table: "UserDetails",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Following",
                table: "UserDetails",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Follower",
                table: "UserDetails");

            migrationBuilder.DropColumn(
                name: "Following",
                table: "UserDetails");
        }
    }
}
