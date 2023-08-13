using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Naturistic.Infrastructure.Migrations.ApplicationIdentityDb
{
    /// <inheritdoc />
    public partial class PasswordRecoveryTokens01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_ConfirmCodes_AspNetUsers_UserId",
            //    table: "ConfirmCodes");

            migrationBuilder.DropIndex(
                name: "IX_ConfirmCodes_UserId",
                table: "ConfirmCodes");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ConfirmCodes",
                type: "varchar(256)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(256)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConfirmCodes_UserId",
                table: "ConfirmCodes",
                column: "UserId",
                unique: true);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_ConfirmCodes_AspNetUsers_UserId",
            //    table: "ConfirmCodes",
            //    column: "UserId",
            //    principalTable: "AspNetUsers",
            //    principalColumn: "Id",
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

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ConfirmCodes",
                type: "varchar(256)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(256)");

            migrationBuilder.CreateIndex(
                name: "IX_ConfirmCodes_UserId",
                table: "ConfirmCodes",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_ConfirmCodes_AspNetUsers_UserId",
            //    table: "ConfirmCodes",
            //    column: "UserId",
            //    principalTable: "AspNetUsers",
            //    principalColumn: "Id");
        }
    }
}
