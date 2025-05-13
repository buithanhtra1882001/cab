using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CabPaymentService.Migrations
{
    public partial class AddPaymentCommission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PaymentCommissionId",
                table: "VnpayTransactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentReceiver",
                table: "VnpayTransactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "PaymentCommission",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    CommissionPercentage = table.Column<decimal>(type: "numeric", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentCommission", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VnpayTransactions_PaymentCommissionId",
                table: "VnpayTransactions",
                column: "PaymentCommissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_VnpayTransactions_PaymentCommission_PaymentCommissionId",
                table: "VnpayTransactions",
                column: "PaymentCommissionId",
                principalTable: "PaymentCommission",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VnpayTransactions_PaymentCommission_PaymentCommissionId",
                table: "VnpayTransactions");

            migrationBuilder.DropTable(
                name: "PaymentCommission");

            migrationBuilder.DropIndex(
                name: "IX_VnpayTransactions_PaymentCommissionId",
                table: "VnpayTransactions");

            migrationBuilder.DropColumn(
                name: "PaymentCommissionId",
                table: "VnpayTransactions");

            migrationBuilder.DropColumn(
                name: "PaymentReceiver",
                table: "VnpayTransactions");
        }
    }
}
