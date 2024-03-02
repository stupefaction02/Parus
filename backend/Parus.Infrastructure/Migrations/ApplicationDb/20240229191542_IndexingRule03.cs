using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parus.Infrastructure.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class IndexingRule03 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IndexingRule",
                table: "Broadcasts",
                newName: "IndexingStatus");

            migrationBuilder.AddColumn<byte>(
                name: "IndexingStatus",
                table: "Categories",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IndexingStatus",
                table: "Categories");

            migrationBuilder.RenameColumn(
                name: "IndexingStatus",
                table: "Broadcasts",
                newName: "IndexingRule");
        }
    }
}
