﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parus.Infrastructure.Migrations.ApplicationIdentityDb
{
    /// <inheritdoc />
    public partial class RebuildConfirmCodeToUserRElation4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddForeignKey(
            //    name: "FK_ConfirmCodes_AspNetUsers_UserId",
            //    table: "ConfirmCodes",
            //    column: "UserId",
            //    principalTable: "AspNetUsers",
            //    principalColumn: "UserName",
            //    onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_ConfirmCodes_AspNetUsers_UserId",
            //    table: "ConfirmCodes");
        }
    }
}