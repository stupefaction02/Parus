using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Parus.Infrastructure.Migrations.ApplicationIdentityDb
{
    /// <inheritdoc />
    public partial class Confirmation_AddUserEmailInsteadOfUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0fa781c7-c74e-41ae-ad4c-621cb510b428");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "89eeade3-25d8-49cf-b3bc-ced203d4ac0a");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8fe196b2-c816-493e-9107-dd13d6cba28e");

            migrationBuilder.RenameColumn(
                name: "IdentityUserId",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                newName: "IdentityUserId");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ChatColor", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "0fa781c7-c74e-41ae-ad4c-621cb510b428", 0, null, "8adacfae-1521-4c23-8dab-f0960a054960", null, false, false, null, null, null, null, null, false, "97c14137-14a3-4050-8ea9-c4ecb2efeb1c", false, "Griffith" },
                    { "89eeade3-25d8-49cf-b3bc-ced203d4ac0a", 0, null, "cb4a7aa7-c81d-4fb6-9058-404f93595e10", null, false, false, null, null, null, null, null, false, "539b24d9-936c-4f78-823b-192f35db6b22", false, "Farzana" },
                    { "8fe196b2-c816-493e-9107-dd13d6cba28e", 0, null, "23f3c97a-83c3-4c02-9684-209c35e0fa50", null, false, false, null, null, null, null, null, false, "5b0733d8-4e96-4016-a99f-eb236f0f7a00", false, "Guts" }
                });
        }
    }
}
