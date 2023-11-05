using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parus.Infrastructure.Migrations.ApplicationIdentityDb
{
    /// <inheritdoc />
    public partial class ConfirmCodesUpdate01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AlterColumn<string>(
            //    name: "UserId",
            //    table: "ConfirmCodes",
            //    type: "nvarchar(450)",
            //    nullable: false,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(256)");

            migrationBuilder.DropForeignKey(
                name: "FK_ConfirmCodes_AspNetUsers_UserId",
                table: "ConfirmCodes");
        }

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
        {
			//migrationBuilder.AlterColumn<string>(
			//    name: "UserId",
			//    table: "ConfirmCodes",
			//    type: "nvarchar(256)",
			//    nullable: false,
			//    oldClrType: typeof(string),
			//    oldType: "nvarchar(450)");

			migrationBuilder.AddForeignKey(
				name: "FK_ConfirmCodes_AspNetUsers_UserId",
				table: "ConfirmCodes",
				column: "UserId",
				principalTable: "AspNetUsers",
				principalColumn: "UserName",
				onDelete: ReferentialAction.Cascade);
		}
	}
}
