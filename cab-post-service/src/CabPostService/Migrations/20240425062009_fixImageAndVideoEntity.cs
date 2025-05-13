using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CabPostService.Migrations
{
    public partial class fixImageAndVideoEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VideoUrls",
                table: "Posts",
                newName: "VideoIds");

            migrationBuilder.RenameColumn(
                name: "ImageUrls",
                table: "Posts",
                newName: "ImageIds");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VideoIds",
                table: "Posts",
                newName: "VideoUrls");

            migrationBuilder.RenameColumn(
                name: "ImageIds",
                table: "Posts",
                newName: "ImageUrls");
        }
    }
}
