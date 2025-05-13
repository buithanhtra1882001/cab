using Microsoft.EntityFrameworkCore.Migrations;

namespace CabUserService.Migrations
{
    public partial class AddUserNameIntoUserTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Users",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Users");
        }
    }
}
