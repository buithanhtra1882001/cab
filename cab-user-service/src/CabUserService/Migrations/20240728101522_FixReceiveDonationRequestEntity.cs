using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CabUserService.Migrations
{
    public partial class FixReceiveDonationRequestEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "DonateReceiverRequests",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateIndex(
                name: "IX_DonateReceiverRequests_UserId",
                table: "DonateReceiverRequests",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_DonateReceiverRequests_Users_UserId",
                table: "DonateReceiverRequests",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DonateReceiverRequests_Users_UserId",
                table: "DonateReceiverRequests");

            migrationBuilder.DropIndex(
                name: "IX_DonateReceiverRequests_UserId",
                table: "DonateReceiverRequests");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "DonateReceiverRequests",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
