using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parus.Infrastructure.Migrations.BillingDb
{
    /// <inheritdoc />
    public partial class Add_Autocontinuation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "ExpiresAt",
                table: "SubscribeSessions",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "Autocontinuation",
                table: "SubscribeSessions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Autocontinuation",
                table: "SubscribeSessions");

            migrationBuilder.AlterColumn<int>(
                name: "ExpiresAt",
                table: "SubscribeSessions",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
