using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CabGroupService.Migrations
{
    public partial class alter_table_group_member_add_column_permissions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Permissions",
                table: "GroupMembers",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Permissions",
                table: "GroupMembers");
        }
    }
}
