using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CabUserService.Migrations
{
    public partial class AddUserFriendAndUpdateFriendAction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserFriends",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    FriendId = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFriends", x => new { x.UserId, x.FriendId });
                });

            migrationBuilder.CreateTable(
                name: "UserRequestFriendActions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    StatusAction = table.Column<int>(type: "integer", nullable: false),
                    AcceptStatus = table.Column<int>(type: "integer", nullable: false),
                    TypeRequest = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRequestFriendActions", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFriends");

            migrationBuilder.DropTable(
                name: "UserRequestFriendActions");
        }
    }
}
