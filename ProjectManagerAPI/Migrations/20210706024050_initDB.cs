using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProjectManagerAPI.Migrations
{
    public partial class initDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdateDate",
                table: "ServerInfos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2021, 7, 6, 2, 40, 50, 162, DateTimeKind.Utc).AddTicks(2751),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2021, 7, 4, 10, 38, 56, 351, DateTimeKind.Utc).AddTicks(9180));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "ServerInfos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2021, 7, 6, 2, 40, 50, 162, DateTimeKind.Utc).AddTicks(2026),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2021, 7, 4, 10, 38, 56, 351, DateTimeKind.Utc).AddTicks(7727));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdateDate",
                table: "ServerInfos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2021, 7, 4, 10, 38, 56, 351, DateTimeKind.Utc).AddTicks(9180),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2021, 7, 6, 2, 40, 50, 162, DateTimeKind.Utc).AddTicks(2751));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "ServerInfos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2021, 7, 4, 10, 38, 56, 351, DateTimeKind.Utc).AddTicks(7727),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2021, 7, 6, 2, 40, 50, 162, DateTimeKind.Utc).AddTicks(2026));
        }
    }
}
