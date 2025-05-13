using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CabUserService.Migrations
{
    public partial class UpdateUserDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Company",
                table: "UserDetails",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HomeLand",
                table: "UserDetails",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsShowCompany",
                table: "UserDetails",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsShowHomeLand",
                table: "UserDetails",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsShowMarry",
                table: "UserDetails",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsShowSchool",
                table: "UserDetails",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsShowSexualOrientation",
                table: "UserDetails",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "School",
                table: "UserDetails",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Company",
                table: "UserDetails");

            migrationBuilder.DropColumn(
                name: "HomeLand",
                table: "UserDetails");

            migrationBuilder.DropColumn(
                name: "IsShowCompany",
                table: "UserDetails");

            migrationBuilder.DropColumn(
                name: "IsShowHomeLand",
                table: "UserDetails");

            migrationBuilder.DropColumn(
                name: "IsShowMarry",
                table: "UserDetails");

            migrationBuilder.DropColumn(
                name: "IsShowSchool",
                table: "UserDetails");

            migrationBuilder.DropColumn(
                name: "IsShowSexualOrientation",
                table: "UserDetails");

            migrationBuilder.DropColumn(
                name: "School",
                table: "UserDetails");
        }
    }
}
