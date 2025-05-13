using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CabUserService.Migrations
{
    public partial class AddTableReceiveDonationRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanReceiveDonation",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "DonateReceiverRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    NationalId = table.Column<string>(type: "text", nullable: true),
                    ReferenceLinks = table.Column<string>(type: "text", nullable: true),
                    BankAccount = table.Column<string>(type: "text", nullable: true),
                    BankName = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonateReceiverRequests", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DonateReceiverRequests");

            migrationBuilder.DropColumn(
                name: "CanReceiveDonation",
                table: "Users");
        }
    }
}
