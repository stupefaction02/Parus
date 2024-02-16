using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add2FAVerifCodes01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable("TwoFAVerificationCodes");

            migrationBuilder.CreateTable(
                name: "TwoFAVerificationCodes",
                columns: table => new
                {
                    ConfirmCodeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Code = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TwoFAVerificationCodes", x => x.ConfirmCodeId);

                    table.ForeignKey(
                    name: "FK_TwoFAVerificationCodes_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("TwoFAVerificationCodes");
            //migrationBuilder.DropForeignKey("FK_TwoFAVerificationCodes_AspNetUsers_UserId", "TwoFAVerificationCodes");
            //migrationBuilder.DropPrimaryKey("PK_TwoFAVerificationCodes", "TwoFAVerificationCodes");
        }
    }
}
