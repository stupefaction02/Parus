using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexingRule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "IndexingRule",
                table: "AspNetUsers",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn("IndexingRule", "AspNetUsers");
        }
    }
}
