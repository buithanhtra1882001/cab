using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CabPaymentService.Migrations
{
    public partial class AddTransactionType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TransactionType",
                table: "VnpayTransactions",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransactionType",
                table: "VnpayTransactions");
        }
    }
}
