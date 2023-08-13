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
            migrationBuilder.AddColumn<string>(
                name: "PasswordRecoveryToken",
                table: "AspNetUsers",
                type: "nvarchar(128)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PasswordRecoveryTokenTimestamp",
                table: "AspNetUsers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordRecoveryToken",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PasswordRecoveryTokenTimestamp",
                table: "AspNetUsers");
        }
    }
}
