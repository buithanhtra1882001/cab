using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CabUserService.Migrations
{
    public partial class AddColumnCoverImageTableUserDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoverImage",
                table: "UserDetails",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverImage",
                table: "UserDetails");
        }
    }
}
