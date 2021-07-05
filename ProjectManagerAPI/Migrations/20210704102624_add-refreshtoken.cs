using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProjectManagerAPI.Migrations
{
    public partial class addrefreshtoken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdateDate",
                table: "ServerInfos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2021, 7, 4, 10, 26, 24, 282, DateTimeKind.Utc).AddTicks(5024),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2021, 7, 3, 9, 28, 28, 213, DateTimeKind.Utc).AddTicks(4042));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "ServerInfos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2021, 7, 4, 10, 26, 24, 282, DateTimeKind.Utc).AddTicks(3984),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2021, 7, 3, 9, 28, 28, 213, DateTimeKind.Utc).AddTicks(2333));

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Expires = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByIp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Revoked = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RevokedByIp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReplacedByToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshToken_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_UserId",
                table: "RefreshToken",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshToken");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdateDate",
                table: "ServerInfos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2021, 7, 3, 9, 28, 28, 213, DateTimeKind.Utc).AddTicks(4042),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2021, 7, 4, 10, 26, 24, 282, DateTimeKind.Utc).AddTicks(5024));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "ServerInfos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2021, 7, 3, 9, 28, 28, 213, DateTimeKind.Utc).AddTicks(2333),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2021, 7, 4, 10, 26, 24, 282, DateTimeKind.Utc).AddTicks(3984));
        }
    }
}
