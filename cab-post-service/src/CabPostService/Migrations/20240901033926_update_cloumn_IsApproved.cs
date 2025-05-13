using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CabPostService.Migrations
{
    public partial class update_cloumn_IsApproved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IsApproved",
                table: "Posts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsApproved",
                table: "Posts",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
