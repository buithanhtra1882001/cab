using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CabPostService.Migrations
{
    public partial class AddTablePostNotifyAdmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PostNotifyAdmin",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    PostId = table.Column<string>(type: "text", nullable: false),
                    IsAcceptHide = table.Column<bool>(type: "boolean", nullable: false),
                    IsHandle = table.Column<bool>(type: "boolean", nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    ReportNumber = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostNotifyAdmin", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostNotifyAdmin");
        }
    }
}
