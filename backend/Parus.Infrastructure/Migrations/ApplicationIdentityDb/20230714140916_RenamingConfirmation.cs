using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Parus.Infrastructure.Migrations.ApplicationIdentityDb
{
    /// <inheritdoc />
    public partial class RenamingConfirmation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "ConfirmCodes",
                columns: table => new
                {
                    ConfirmCodeEntityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdentityUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfirmCodes", x => x.ConfirmCodeEntityId);
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfirmCodes");

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

            migrationBuilder.CreateTable(
                name: "ConfirmDigits",
                columns: table => new
                {
                    ConfirmDigitsEntryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConfirmDigit = table.Column<int>(type: "int", nullable: false),
                    IdentityUserId = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
    }
}
