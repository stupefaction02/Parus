using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Parus.Infrastructure.Migrations.ApplicationIdentityDb
{
    /// <inheritdoc />
    public partial class RebuildConfirmCodeToUserRElation2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e04b8aa-1024-4207-a0ad-bcb8b5a11888");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8217b76d-e9e2-4fd1-a4b5-ca72f2eb16e1");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b925e4a4-8171-4355-aa76-804c58e601bc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ChatColor", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "7e04b8aa-1024-4207-a0ad-bcb8b5a11888", 0, null, "e1dbe1f4-da32-45a5-9452-20d4d7d26be7", null, false, false, null, null, null, null, null, false, "663bcb73-90b7-44dc-9079-fe306eaef1be", false, "Guts" },
                    { "8217b76d-e9e2-4fd1-a4b5-ca72f2eb16e1", 0, null, "e287c1ab-83d3-45f3-9fe4-e7e6e9fcd575", null, false, false, null, null, null, null, null, false, "06cc16c3-e021-4b03-848e-e91ccabb969d", false, "Farzana" },
                    { "b925e4a4-8171-4355-aa76-804c58e601bc", 0, null, "399dbaec-2b54-4b46-acb1-1d1840b8804d", null, false, false, null, null, null, null, null, false, "74894346-fe17-4caf-9846-f388dabbc3d2", false, "Griffith" }
                });
        }
    }
}
