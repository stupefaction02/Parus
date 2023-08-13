using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Naturistic.Infrastructure.Migrations.ApplicationIdentityDb
{
    /// <inheritdoc />
    public partial class PasswordRecoveryTokens04 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint("UC_AspNetUsers_UserName", "AspNetUsers", "UserName");

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
            migrationBuilder.DropUniqueConstraint("UC_AspNetUsers_UserName", "AspNetUsers");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_ConfirmCodes_AspNetUsers_UserId",
            //    table: "ConfirmCodes");
        }
    }
}
