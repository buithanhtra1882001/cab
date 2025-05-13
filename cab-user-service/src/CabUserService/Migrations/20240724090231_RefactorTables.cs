using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CabUserService.Migrations
{
    public partial class RefactorTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Balance",
                table: "UserDetails");

            migrationBuilder.AlterColumn<double>(
                name: "PlatformFee",
                table: "UserTransactionLogs",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<double>(
                name: "Amount",
                table: "UserTransactionLogs",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<double>(
                name: "Coin",
                table: "Users",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<double>(
                name: "Amount",
                table: "UserBalanceLogs",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "PlatformFee",
                table: "UserTransactionLogs",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "UserTransactionLogs",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<long>(
                name: "Coin",
                table: "Users",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "UserDetails",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "UserBalanceLogs",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");
        }
    }
}
