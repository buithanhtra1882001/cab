using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CabUserService.Migrations
{
    public partial class AddNewColumnsTableUserTransaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DonationType",
                table: "UserTransactionLogs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "PostId",
                table: "UserTransactionLogs",
                type: "uuid",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DonationType",
                table: "UserTransactionLogs");

            migrationBuilder.DropColumn(
                name: "PostId",
                table: "UserTransactionLogs");
        }
    }
}
