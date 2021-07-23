using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProjectManagerAPI.Migrations
{
    public partial class rftoken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshToken_AspNetUsers_UserId",
                table: "RefreshToken");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RefreshToken",
                table: "RefreshToken");

            migrationBuilder.RenameTable(
                name: "RefreshToken",
                newName: "RefreshTokens");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshToken_UserId",
                table: "RefreshTokens",
                newName: "IX_RefreshTokens_UserId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdateDate",
                table: "ServerInfos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2021, 7, 19, 13, 42, 4, 1, DateTimeKind.Utc).AddTicks(5733),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2021, 7, 18, 8, 58, 17, 404, DateTimeKind.Utc).AddTicks(4206));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "ServerInfos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2021, 7, 19, 13, 42, 4, 1, DateTimeKind.Utc).AddTicks(3883),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2021, 7, 18, 8, 58, 17, 404, DateTimeKind.Utc).AddTicks(2639));

            migrationBuilder.AddPrimaryKey(
                name: "PK_RefreshTokens",
                table: "RefreshTokens",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_AspNetUsers_UserId",
                table: "RefreshTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_AspNetUsers_UserId",
                table: "RefreshTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RefreshTokens",
                table: "RefreshTokens");

            migrationBuilder.RenameTable(
                name: "RefreshTokens",
                newName: "RefreshToken");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshToken",
                newName: "IX_RefreshToken_UserId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdateDate",
                table: "ServerInfos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2021, 7, 18, 8, 58, 17, 404, DateTimeKind.Utc).AddTicks(4206),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2021, 7, 19, 13, 42, 4, 1, DateTimeKind.Utc).AddTicks(5733));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "ServerInfos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2021, 7, 18, 8, 58, 17, 404, DateTimeKind.Utc).AddTicks(2639),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2021, 7, 19, 13, 42, 4, 1, DateTimeKind.Utc).AddTicks(3883));

            migrationBuilder.AddPrimaryKey(
                name: "PK_RefreshToken",
                table: "RefreshToken",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshToken_AspNetUsers_UserId",
                table: "RefreshToken",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
