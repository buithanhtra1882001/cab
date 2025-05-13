using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CabPostService.Migrations
{
    public partial class UpdateTableSharePostAndUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SharedUserId",
                table: "SharePosts",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SharePosts_SharedUserId",
                table: "SharePosts",
                column: "SharedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SharePosts_Users_SharedUserId",
                table: "SharePosts",
                column: "SharedUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SharePosts_Users_SharedUserId",
                table: "SharePosts");

            migrationBuilder.DropIndex(
                name: "IX_SharePosts_SharedUserId",
                table: "SharePosts");

            migrationBuilder.DropColumn(
                name: "SharedUserId",
                table: "SharePosts");
        }
    }
}
