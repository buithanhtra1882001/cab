using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CabPostService.Migrations
{
    public partial class alter_table_post_add_column_IsApproved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Posts",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Posts");
        }
    }
}
