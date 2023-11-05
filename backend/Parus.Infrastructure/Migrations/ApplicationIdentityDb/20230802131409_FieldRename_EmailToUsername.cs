using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Parus.Infrastructure.Migrations.ApplicationIdentityDb
{
    /// <inheritdoc />
    public partial class FieldRename_EmailToUsername : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "27b9b46f-83b8-4a4b-b9eb-79f689034b90");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "445d65df-295a-47ec-bb2c-80c96ac2b33d");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "97666bf3-4d4d-41a6-a24b-505900799f7b");

            migrationBuilder.RenameColumn(
                name: "UserEmail",
                table: "ConfirmCodes",
                newName: "Username");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ChatColor", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "19283991-b3ad-4780-9ed5-fc7e17d4e2a6", 0, null, "ea7560d8-7236-4f3b-bac8-5405ca698438", null, false, false, null, null, null, null, null, false, "ce109fbe-3bd2-4351-a83c-73be27268286", false, "Farzana" },
                    { "7a9225b8-6f29-47a8-aa0e-7de72b4dee33", 0, null, "a3aec899-0e8e-4b47-9a2a-b3e7d09c89e0", null, false, false, null, null, null, null, null, false, "1d4f1a6e-9c21-43cf-ba8f-695973fb0a65", false, "Griffith" },
                    { "d8e7f2a4-bd0e-41d1-9429-f9a90cac23ed", 0, null, "14127725-389a-4427-8db0-8a7870cbcc5e", null, false, false, null, null, null, null, null, false, "993d093e-959a-416b-b244-900c957460cb", false, "Guts" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "19283991-b3ad-4780-9ed5-fc7e17d4e2a6");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7a9225b8-6f29-47a8-aa0e-7de72b4dee33");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d8e7f2a4-bd0e-41d1-9429-f9a90cac23ed");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "ConfirmCodes",
                newName: "UserEmail");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ChatColor", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "27b9b46f-83b8-4a4b-b9eb-79f689034b90", 0, null, "0e58b1a7-0214-492a-8d99-5f14e7438522", null, false, false, null, null, null, null, null, false, "26f423e6-f226-4aa0-bf4b-371e3e40daeb", false, "Griffith" },
                    { "445d65df-295a-47ec-bb2c-80c96ac2b33d", 0, null, "5d9e7826-31a9-4914-9011-ec7e0db64573", null, false, false, null, null, null, null, null, false, "ed3cdc53-ace0-470e-9c5d-d64f7151406c", false, "Farzana" },
                    { "97666bf3-4d4d-41a6-a24b-505900799f7b", 0, null, "4be9c8bd-c8df-464f-9d42-71ec16b338b7", null, false, false, null, null, null, null, null, false, "a682ac3f-cdf2-4efc-860f-06d7e4783409", false, "Guts" }
                });
        }
    }
}
