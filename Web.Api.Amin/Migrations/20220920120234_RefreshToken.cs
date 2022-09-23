using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web.Api.Amin.Migrations
{
    public partial class RefreshToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TokenExe",
                table: "UserTokens",
                newName: "TokenExp");

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "UserTokens",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExp",
                table: "UserTokens",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "UserTokens");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExp",
                table: "UserTokens");

            migrationBuilder.RenameColumn(
                name: "TokenExp",
                table: "UserTokens",
                newName: "TokenExe");
        }
    }
}
