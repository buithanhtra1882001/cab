using Microsoft.EntityFrameworkCore.Migrations;

namespace WCABNetwork.Cab.IdentityService.Migrations
{
    public partial class AddSoftdeletedColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSoftDeleted",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSoftDeleted",
                table: "AspNetUsers");
        }
    }
}
