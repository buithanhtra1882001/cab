using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CabUserService.Migrations
{
    public partial class AddDeleteCascadeUserDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Balance",
                table: "Users",
                newName: "Coin");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Coin",
                table: "Users",
                newName: "Balance");
        }
    }
}
