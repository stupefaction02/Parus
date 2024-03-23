using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parus.Infrastructure.Migrations.BillingDb
{
    /// <inheritdoc />
    public partial class Init1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubscribeProfiles",
                columns: table => new
                {
                    Key = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscribeProfiles", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "SubscribeSessions",
                columns: table => new
                {
                    Key = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfileKey = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ExpiresAt = table.Column<int>(type: "int", nullable: false),
                    PurchaserUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubjectUserId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscribeSessions", x => x.Key);
                    table.ForeignKey(
                        name: "FK_SubscribeSessions_SubscribeProfiles_ProfileKey",
                        column: x => x.ProfileKey,
                        principalTable: "SubscribeProfiles",
                        principalColumn: "Key");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubscribeSessions_ProfileKey",
                table: "SubscribeSessions",
                column: "ProfileKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubscribeSessions");

            migrationBuilder.DropTable(
                name: "SubscribeProfiles");
        }
    }
}
