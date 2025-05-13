using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CabPostService.Migrations
{
    public partial class UpdateSharePost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SharePosts_Users_SharedUserId",
                table: "SharePosts");

            migrationBuilder.AlterColumn<Guid>(
                name: "SharedUserId",
                table: "SharePosts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SharePosts_Users_SharedUserId",
                table: "SharePosts",
                column: "SharedUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SharePosts_Users_SharedUserId",
                table: "SharePosts");

            migrationBuilder.AlterColumn<Guid>(
                name: "SharedUserId",
                table: "SharePosts",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_SharePosts_Users_SharedUserId",
                table: "SharePosts",
                column: "SharedUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
