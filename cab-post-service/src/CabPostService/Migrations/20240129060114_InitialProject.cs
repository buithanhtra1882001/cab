using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CabPostService.Migrations
{
    public partial class InitialProject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PostCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Slug = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Thumbnail = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Score = table.Column<decimal>(type: "numeric", nullable: false),
                    IsSoftDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PostCategoryType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PostCategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostCategoryType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AdminBoost = table.Column<bool>(type: "boolean", nullable: false),
                    PostType = table.Column<string>(type: "text", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Hashtag_Ids = table.Column<Guid[]>(type: "uuid[]", nullable: false),
                    Hashtags = table.Column<string[]>(type: "text[]", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    ImageUrls = table.Column<string[]>(type: "text[]", nullable: false),
                    VideoUrls = table.Column<string[]>(type: "text[]", nullable: false),
                    Point = table.Column<int>(type: "integer", nullable: false),
                    LikesCount = table.Column<int>(type: "integer", nullable: false),
                    CommentsCount = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    IsDonateOpen = table.Column<bool>(type: "boolean", nullable: false),
                    IsChecked = table.Column<bool>(type: "boolean", nullable: false),
                    IsSoftDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    ViewCount = table.Column<int>(type: "integer", nullable: false),
                    VoteUpCount = table.Column<int>(type: "integer", nullable: false),
                    VoteDownCount = table.Column<int>(type: "integer", nullable: false),
                    PosterScore = table.Column<decimal>(type: "numeric", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PostVideos",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    PostId = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    MediaVideoId = table.Column<string>(type: "text", nullable: false),
                    VideoUrl = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    LengthVideo = table.Column<double>(type: "double precision", nullable: false),
                    AvgViewLength = table.Column<double>(type: "double precision", nullable: false),
                    ViewCount = table.Column<int>(type: "integer", nullable: false),
                    IsViolence = table.Column<bool>(type: "boolean", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostVideos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SharePosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PostId = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ShareLink = table.Column<string>(type: "text", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SharePosts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Fullname = table.Column<string>(type: "text", nullable: false),
                    Avatar = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PostVotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PostId = table.Column<string>(type: "text", nullable: false),
                    UserVoteId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostVotes", x => new { x.Id, x.PostId, x.UserVoteId });
                    table.ForeignKey(
                        name: "FK_PostVotes_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostUsers",
                columns: table => new
                {
                    PostId = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostUsers", x => new { x.PostId, x.UserId });
                    table.ForeignKey(
                        name: "FK_PostUsers_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostUsers_UserId",
                table: "PostUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PostVotes_PostId",
                table: "PostVotes",
                column: "PostId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostCategories");

            migrationBuilder.DropTable(
                name: "PostCategoryType");

            migrationBuilder.DropTable(
                name: "PostUsers");

            migrationBuilder.DropTable(
                name: "PostVideos");

            migrationBuilder.DropTable(
                name: "PostVotes");

            migrationBuilder.DropTable(
                name: "SharePosts");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Posts");
        }
    }
}
