using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProjectManagerAPI.Migrations
{
    public partial class removeunique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tasks_Name",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Reports_Name",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Projects_Name",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Phases_Name",
                table: "Phases");

            migrationBuilder.DropIndex(
                name: "IX_GroupTypes_Name",
                table: "GroupTypes");

            migrationBuilder.DropIndex(
                name: "IX_Groups_Name",
                table: "Groups");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdateDate",
                table: "ServerInfos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2021, 7, 4, 10, 38, 56, 351, DateTimeKind.Utc).AddTicks(9180),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2021, 7, 4, 10, 26, 24, 282, DateTimeKind.Utc).AddTicks(5024));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "ServerInfos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2021, 7, 4, 10, 38, 56, 351, DateTimeKind.Utc).AddTicks(7727),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2021, 7, 4, 10, 26, 24, 282, DateTimeKind.Utc).AddTicks(3984));

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_Name",
                table: "Tasks",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_Name",
                table: "Reports",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Name",
                table: "Projects",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Phases_Name",
                table: "Phases",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_GroupTypes_Name",
                table: "GroupTypes",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_Name",
                table: "Groups",
                column: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tasks_Name",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Reports_Name",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Projects_Name",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Phases_Name",
                table: "Phases");

            migrationBuilder.DropIndex(
                name: "IX_GroupTypes_Name",
                table: "GroupTypes");

            migrationBuilder.DropIndex(
                name: "IX_Groups_Name",
                table: "Groups");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdateDate",
                table: "ServerInfos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2021, 7, 4, 10, 26, 24, 282, DateTimeKind.Utc).AddTicks(5024),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2021, 7, 4, 10, 38, 56, 351, DateTimeKind.Utc).AddTicks(9180));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "ServerInfos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2021, 7, 4, 10, 26, 24, 282, DateTimeKind.Utc).AddTicks(3984),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2021, 7, 4, 10, 38, 56, 351, DateTimeKind.Utc).AddTicks(7727));

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_Name",
                table: "Tasks",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_Name",
                table: "Reports",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Name",
                table: "Projects",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Phases_Name",
                table: "Phases",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_GroupTypes_Name",
                table: "GroupTypes",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_Name",
                table: "Groups",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");
        }
    }
}
