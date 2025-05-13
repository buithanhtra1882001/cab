using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CabPostService.Migrations
{
    public partial class DeleteHashtagIds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hashtag_Ids",
                table: "Posts");

            migrationBuilder.AlterColumn<string>(
                name: "Hashtags",
                table: "Posts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string[]),
                oldType: "text[]");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string[]>(
                name: "Hashtags",
                table: "Posts",
                type: "text[]",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<Guid[]>(
                name: "Hashtag_Ids",
                table: "Posts",
                type: "uuid[]",
                nullable: false,
                defaultValue: new Guid[0]);
        }
    }
}
