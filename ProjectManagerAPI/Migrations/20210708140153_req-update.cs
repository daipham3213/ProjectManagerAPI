using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProjectManagerAPI.Migrations
{
    public partial class requpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdateDate",
                table: "ServerInfos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2021, 7, 8, 14, 1, 52, 932, DateTimeKind.Utc).AddTicks(3948),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2021, 7, 7, 15, 2, 47, 438, DateTimeKind.Utc).AddTicks(1742));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "ServerInfos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2021, 7, 8, 14, 1, 52, 932, DateTimeKind.Utc).AddTicks(2437),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2021, 7, 7, 15, 2, 47, 438, DateTimeKind.Utc).AddTicks(370));

            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Value",
                table: "Requests");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdateDate",
                table: "ServerInfos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2021, 7, 7, 15, 2, 47, 438, DateTimeKind.Utc).AddTicks(1742),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2021, 7, 8, 14, 1, 52, 932, DateTimeKind.Utc).AddTicks(3948));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "ServerInfos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2021, 7, 7, 15, 2, 47, 438, DateTimeKind.Utc).AddTicks(370),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2021, 7, 8, 14, 1, 52, 932, DateTimeKind.Utc).AddTicks(2437));
        }
    }
}
