using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add2FA_CustomerKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TwoFactoryCustomerKeys",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(72)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TwoFactoryCustomerKeys", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_TwoFactoryCustomerKeys_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("TwoFactoryCustomerKeys");
        }
    }
}
