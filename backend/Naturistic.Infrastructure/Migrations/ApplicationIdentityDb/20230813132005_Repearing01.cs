﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Naturistic.Infrastructure.Migrations.ApplicationIdentityDb
{
    /// <inheritdoc />
    public partial class Repearing01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint("UC_AspNetUsers_UserName", "AspNetUsers");

            //migrationBuilder.AlterColumn<string>(
            //    name: "UserName",
            //    table: "AspNetUsers",
            //    type: "nvarchar(256)",
            //    maxLength: 256,
            //    nullable: false,
            //    defaultValue: "",
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(256)",
            //    oldMaxLength: 256,
            //    oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint("UC_AspNetUsers_UserName", "AspNetUsers", "UserName");

            //migrationBuilder.AlterColumn<string>(
            //    name: "UserName",
            //    table: "AspNetUsers",
            //    type: "nvarchar(256)",
            //    maxLength: 256,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(256)",
            //    oldMaxLength: 256);
        }
    }
}