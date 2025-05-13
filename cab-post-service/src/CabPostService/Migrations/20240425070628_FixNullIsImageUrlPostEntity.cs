using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CabPostService.Migrations
{
    public partial class FixNullIsImageUrlPostEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string[]>(
                name: "VideoIds",
                table: "Posts",
                type: "text[]",
                nullable: true,
                oldClrType: typeof(string[]),
                oldType: "text[]");

            migrationBuilder.AlterColumn<string[]>(
                name: "ImageIds",
                table: "Posts",
                type: "text[]",
                nullable: true,
                oldClrType: typeof(string[]),
                oldType: "text[]");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string[]>(
                name: "VideoIds",
                table: "Posts",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0],
                oldClrType: typeof(string[]),
                oldType: "text[]",
                oldNullable: true);

            migrationBuilder.AlterColumn<string[]>(
                name: "ImageIds",
                table: "Posts",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0],
                oldClrType: typeof(string[]),
                oldType: "text[]",
                oldNullable: true);
        }
    }
}
