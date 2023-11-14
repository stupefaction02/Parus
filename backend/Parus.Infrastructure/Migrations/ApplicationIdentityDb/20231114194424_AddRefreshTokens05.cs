using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parus.Infrastructure.Migrations.ApplicationIdentityDb
{
    /// <inheritdoc />
    public partial class AddRefreshTokens05 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_RefreshSessions_RefreshSessionRefreshTokenId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_RefreshSessionRefreshTokenId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RefreshSessionRefreshTokenId",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshSessions_UserId",
                table: "RefreshSessions",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshSessions_AspNetUsers_UserId",
                table: "RefreshSessions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshSessions_AspNetUsers_UserId",
                table: "RefreshSessions");

            migrationBuilder.DropIndex(
                name: "IX_RefreshSessions_UserId",
                table: "RefreshSessions");

            migrationBuilder.AddColumn<int>(
                name: "RefreshSessionRefreshTokenId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_RefreshSessionRefreshTokenId",
                table: "AspNetUsers",
                column: "RefreshSessionRefreshTokenId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_RefreshSessions_RefreshSessionRefreshTokenId",
                table: "AspNetUsers",
                column: "RefreshSessionRefreshTokenId",
                principalTable: "RefreshSessions",
                principalColumn: "RefreshTokenId");
        }
    }
}
