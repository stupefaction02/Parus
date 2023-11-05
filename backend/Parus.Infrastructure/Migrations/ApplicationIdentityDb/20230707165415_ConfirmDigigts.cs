using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Parus.Infrastructure.Migrations.ApplicationIdentityDb
{
    /// <inheritdoc />
    public partial class ConfirmDigigts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "52b19553-ae84-4daa-9ea7-ed3c6231597e");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8c19b273-1243-4d64-a820-da152dddc098");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9ab64d0d-e286-4c24-8c9a-af75f187249c");

            migrationBuilder.CreateTable(
                name: "ConfirmDigits",
                columns: table => new
                {
                    ConfirmDigitsEntryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdentityUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConfirmDigit = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfirmDigits", x => x.ConfirmDigitsEntryId);
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ChatColor", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "0e7c631d-a35d-4797-8c35-9d0633bb7259", 0, null, "1d507975-05cf-4932-ab6d-d93aa7dcb2dd", null, false, false, null, null, null, null, null, false, "d877a268-ec8f-4414-954f-3cd7f793b0fb", false, "Guts" },
                    { "71a388ef-9c14-40ae-b7d8-2c97b1d45936", 0, null, "08f76bf9-cfbb-45e8-beab-28023cfd7151", null, false, false, null, null, null, null, null, false, "abd33d0a-9c8d-41d6-ae99-525e61f06068", false, "Griffith" },
                    { "e00f1b09-2596-461f-b5db-264f1501f2ca", 0, null, "0c4ebc35-b76e-4a23-8e86-5199a50603cc", null, false, false, null, null, null, null, null, false, "13205066-4bf6-4668-b86f-4b6197ee18ef", false, "Farzana" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfirmDigits");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0e7c631d-a35d-4797-8c35-9d0633bb7259");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "71a388ef-9c14-40ae-b7d8-2c97b1d45936");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e00f1b09-2596-461f-b5db-264f1501f2ca");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ChatColor", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "52b19553-ae84-4daa-9ea7-ed3c6231597e", 0, null, "1c977399-685a-4cb4-8d1a-d80ebe1a1cdf", null, false, false, null, null, null, null, null, false, "a03d38d1-6dd3-4ce8-8740-1f90096874d3", false, "Farzana" },
                    { "8c19b273-1243-4d64-a820-da152dddc098", 0, null, "0ac15898-98e5-41ff-8d30-6450bf1e8b6b", null, false, false, null, null, null, null, null, false, "40af9b83-b34d-4664-8488-60da9fe163bc", false, "Guts" },
                    { "9ab64d0d-e286-4c24-8c9a-af75f187249c", 0, null, "e87746b4-b49a-4de2-9d9f-5c5296ac899c", null, false, false, null, null, null, null, null, false, "1b72a38f-68c6-49aa-b0ae-1876ab2dc8f8", false, "Griffith" }
                });
        }
    }
}
