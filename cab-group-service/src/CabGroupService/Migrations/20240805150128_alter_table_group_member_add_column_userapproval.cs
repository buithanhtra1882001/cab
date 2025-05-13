using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CabGroupService.Migrations
{
    public partial class alter_table_group_member_add_column_userapproval : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserDecentralization",
                table: "GroupMembers",
                newName: "UserApproval");

            migrationBuilder.AddColumn<Guid>(
                name: "DecentralizationUser",
                table: "GroupMembers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DecentralizationUser",
                table: "GroupMembers");

            migrationBuilder.RenameColumn(
                name: "UserApproval",
                table: "GroupMembers",
                newName: "UserDecentralization");
        }
    }
}
