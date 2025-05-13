using Microsoft.EntityFrameworkCore.Migrations;

namespace CabUserService.Migrations
{
    public partial class InitUserDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    SequenceId = table.Column<int>(type: "integer", nullable: false),
                    Credit = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    IsSoftDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
