using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CabPostService.Migrations
{
    public partial class AddColumnTypeShowPostAndTotalReportOfPostAndIsShowPostOfPostUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsShowPost",
                table: "PostUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TotalReport",
                table: "Posts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TypeShowPost",
                table: "Posts",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsShowPost",
                table: "PostUsers");

            migrationBuilder.DropColumn(
                name: "TotalReport",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "TypeShowPost",
                table: "Posts");
        }
    }
}
