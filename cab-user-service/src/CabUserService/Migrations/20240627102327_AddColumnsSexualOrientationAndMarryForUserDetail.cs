using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CabUserService.Migrations
{
    public partial class AddColumnsSexualOrientationAndMarryForUserDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Marry",
                table: "UserDetails",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SexualOrientation",
                table: "UserDetails",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Marry",
                table: "UserDetails");

            migrationBuilder.DropColumn(
                name: "SexualOrientation",
                table: "UserDetails");
        }
    }
}
