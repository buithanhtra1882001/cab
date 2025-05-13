using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CabUserService.Migrations
{
    public partial class UpdateUserType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("0b4fa388-c5aa-4007-9513-39cd74ae7af8"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("144fe031-e466-4929-925e-9bd80b48050c"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("3d6b2b4e-86c7-43ea-8838-7a5fd79acaf4"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("5b1842d3-eff4-4698-be8a-2af194b6e09d"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("60d05c18-c3bd-41a8-97af-c7c20912097e"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("944f9217-a76c-4763-9e9a-5d29ba9d4625"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("a9df8aec-4892-4d2c-a18c-1c3373df6f78"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("a9e7a303-4fcf-4ecf-8a1f-b498b737f45c"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("aabbabbd-5217-40c7-8364-010695e1b643"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("b399da13-47e0-474b-8957-919b2a9ad25e"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("cd63724c-e5a3-48a2-a023-aefbacc6d694"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("f27e895e-29b8-48ed-a35c-9ef982ed9539"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("fd69f597-3355-4213-9c3e-379ac859fa9c"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Avatar", "CreatedAt", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("0b4fa388-c5aa-4007-9513-39cd74ae7af8"), "the-thao.jpg", new DateTime(2023, 10, 10, 8, 53, 2, 301, DateTimeKind.Utc).AddTicks(4632), "Thể thao", new DateTime(2023, 10, 10, 8, 53, 2, 301, DateTimeKind.Utc).AddTicks(4633) },
                    { new Guid("144fe031-e466-4929-925e-9bd80b48050c"), "game.jpg", new DateTime(2023, 10, 10, 8, 53, 2, 301, DateTimeKind.Utc).AddTicks(4641), "Game", new DateTime(2023, 10, 10, 8, 53, 2, 301, DateTimeKind.Utc).AddTicks(4642) },
                    { new Guid("3d6b2b4e-86c7-43ea-8838-7a5fd79acaf4"), "phim.jpg", new DateTime(2023, 10, 10, 8, 53, 2, 301, DateTimeKind.Utc).AddTicks(4655), "Phim", new DateTime(2023, 10, 10, 8, 53, 2, 301, DateTimeKind.Utc).AddTicks(4656) },
                    { new Guid("5b1842d3-eff4-4698-be8a-2af194b6e09d"), "giai-tri-tong-hop.jpg", new DateTime(2023, 10, 10, 8, 53, 2, 301, DateTimeKind.Utc).AddTicks(4636), "Giải trí tổng hợp", new DateTime(2023, 10, 10, 8, 53, 2, 301, DateTimeKind.Utc).AddTicks(4636) },
                    { new Guid("60d05c18-c3bd-41a8-97af-c7c20912097e"), "nswl.PNG", new DateTime(2023, 10, 10, 8, 53, 2, 301, DateTimeKind.Utc).AddTicks(4662), "NSFW ", new DateTime(2023, 10, 10, 8, 53, 2, 301, DateTimeKind.Utc).AddTicks(4663) },
                    { new Guid("944f9217-a76c-4763-9e9a-5d29ba9d4625"), "cong-nghe.jpg", new DateTime(2023, 10, 10, 8, 53, 2, 301, DateTimeKind.Utc).AddTicks(4638), "Công nghệ", new DateTime(2023, 10, 10, 8, 53, 2, 301, DateTimeKind.Utc).AddTicks(4638) },
                    { new Guid("a9df8aec-4892-4d2c-a18c-1c3373df6f78"), "real-life.jpg", new DateTime(2023, 10, 10, 8, 53, 2, 301, DateTimeKind.Utc).AddTicks(4664), "Real life", new DateTime(2023, 10, 10, 8, 53, 2, 301, DateTimeKind.Utc).AddTicks(4664) },
                    { new Guid("a9e7a303-4fcf-4ecf-8a1f-b498b737f45c"), "thu-cung.jpg", new DateTime(2023, 10, 10, 8, 53, 2, 301, DateTimeKind.Utc).AddTicks(4640), "Thú cưng", new DateTime(2023, 10, 10, 8, 53, 2, 301, DateTimeKind.Utc).AddTicks(4640) },
                    { new Guid("aabbabbd-5217-40c7-8364-010695e1b643"), "chu-de-nguoi-di-lam.jpg", new DateTime(2023, 10, 10, 8, 53, 2, 301, DateTimeKind.Utc).AddTicks(4659), "Chủ đề người đi làm ", new DateTime(2023, 10, 10, 8, 53, 2, 301, DateTimeKind.Utc).AddTicks(4659) },
                    { new Guid("b399da13-47e0-474b-8957-919b2a9ad25e"), "chu-de-nguoi-o-nha.jpg", new DateTime(2023, 10, 10, 8, 53, 2, 301, DateTimeKind.Utc).AddTicks(4661), "Chủ đề ở nhà", new DateTime(2023, 10, 10, 8, 53, 2, 301, DateTimeKind.Utc).AddTicks(4661) },
                    { new Guid("cd63724c-e5a3-48a2-a023-aefbacc6d694"), "sang-tao.jpg", new DateTime(2023, 10, 10, 8, 53, 2, 301, DateTimeKind.Utc).AddTicks(4657), "Sáng tạo", new DateTime(2023, 10, 10, 8, 53, 2, 301, DateTimeKind.Utc).AddTicks(4657) },
                    { new Guid("f27e895e-29b8-48ed-a35c-9ef982ed9539"), "food.jpg", new DateTime(2023, 10, 10, 8, 53, 2, 301, DateTimeKind.Utc).AddTicks(4668), "Food", new DateTime(2023, 10, 10, 8, 53, 2, 301, DateTimeKind.Utc).AddTicks(4668) },
                    { new Guid("fd69f597-3355-4213-9c3e-379ac859fa9c"), "anime-wibu.jpg", new DateTime(2023, 10, 10, 8, 53, 2, 301, DateTimeKind.Utc).AddTicks(4666), "Anime - wibu", new DateTime(2023, 10, 10, 8, 53, 2, 301, DateTimeKind.Utc).AddTicks(4666) }
                });
        }
    }
}
