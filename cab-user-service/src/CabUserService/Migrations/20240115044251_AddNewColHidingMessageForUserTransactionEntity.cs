using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CabUserService.Migrations
{
    public partial class AddNewColHidingMessageForUserTransactionEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsHidingMessage",
                table: "UserTransactionLogs",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsHidingMessage",
                table: "UserTransactionLogs");
        }
    }
}
