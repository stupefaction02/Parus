using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Naturistic.Infrastructure.Migrations.ApplicationIdentityDb
{
    /// <inheritdoc />
    public partial class RebuildConfirmCodeToUserRElation1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "ConfirmCodes",
                columns: table => new
                {
                    ConfirmCodeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConfirmCode = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfirmCode", x => x.ConfirmCodeId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConfirmCodes_UserId",
                table: "ConfirmCodes",
                column: "ConfirmCodeId",
                unique: true);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_ConfirmCodes_AspNetUsers_UserId",
            //    table: "ConfirmCodes",
            //    column: "UserId",
            //    principalTable: "AspNetUsers",
            //    principalColumn: "UserName",
            //    onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_ConfirmCodes_AspNetUsers_UserId",
            //    table: "ConfirmCodes");

            migrationBuilder.DropIndex(
                name: "IX_ConfirmCodes_UserId",
                table: "ConfirmCodes");

            migrationBuilder.DropTable("ConfirmCodes");
        }
    }
}
