using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CabUserService.Migrations
{
    public partial class UpdateUserDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsShowCity",
                table: "UserDetails",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsShowDescription",
                table: "UserDetails",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "IsShowDob",
                table: "UserDetails",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsShowEmail",
                table: "UserDetails",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsShowPhone",
                table: "UserDetails",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsShowCity",
                table: "UserDetails");

            migrationBuilder.DropColumn(
                name: "IsShowDescription",
                table: "UserDetails");

            migrationBuilder.DropColumn(
                name: "IsShowDob",
                table: "UserDetails");

            migrationBuilder.DropColumn(
                name: "IsShowEmail",
                table: "UserDetails");

            migrationBuilder.DropColumn(
                name: "IsShowPhone",
                table: "UserDetails");
        }
    }
}
