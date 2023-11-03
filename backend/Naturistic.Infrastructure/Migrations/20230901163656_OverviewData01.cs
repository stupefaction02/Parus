using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OverviewData01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Broadcasts_BroadcastId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Broadcasts_BroadcastId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_BroadcastId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Categories_BroadcastId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "BroadcastId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "BroadcastId",
                table: "Categories");

            migrationBuilder.AddColumn<int>(
                name: "BroadcastInfoId",
                table: "Tags",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Broadcasts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_BroadcastInfoId",
                table: "Tags",
                column: "BroadcastInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_Broadcasts_CategoryId",
                table: "Broadcasts",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Broadcasts_Categories_CategoryId",
                table: "Broadcasts",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Broadcasts_BroadcastInfoId",
                table: "Tags",
                column: "BroadcastInfoId",
                principalTable: "Broadcasts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Broadcasts_Categories_CategoryId",
                table: "Broadcasts");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Broadcasts_BroadcastInfoId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_BroadcastInfoId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Broadcasts_CategoryId",
                table: "Broadcasts");

            migrationBuilder.DropColumn(
                name: "BroadcastInfoId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Broadcasts");

            migrationBuilder.AddColumn<int>(
                name: "BroadcastId",
                table: "Tags",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BroadcastId",
                table: "Categories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_BroadcastId",
                table: "Tags",
                column: "BroadcastId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_BroadcastId",
                table: "Categories",
                column: "BroadcastId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Broadcasts_BroadcastId",
                table: "Categories",
                column: "BroadcastId",
                principalTable: "Broadcasts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Broadcasts_BroadcastId",
                table: "Tags",
                column: "BroadcastId",
                principalTable: "Broadcasts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
