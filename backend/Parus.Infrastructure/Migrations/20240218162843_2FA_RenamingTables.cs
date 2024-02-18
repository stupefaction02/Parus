using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _2FA_RenamingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "TwoFAVerificationCodes",
                newName: "TwoFactoryVerificationCodes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "TwoFactoryVerificationCodes",
                newName: "TwoFAVerificationCodes");
        }
    }
}
