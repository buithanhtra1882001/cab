using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CabUserService.Migrations
{
    public partial class UserTransaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Balance",
                table: "UserDetails",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateTable(
                name: "UserBalanceLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBalanceLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserTransactionLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    FromUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ToUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTransactionLogs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserBalanceLogs");

            migrationBuilder.DropTable(
                name: "UserTransactionLogs");

            migrationBuilder.AlterColumn<int>(
                name: "Balance",
                table: "UserDetails",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");
        }
    }
}
