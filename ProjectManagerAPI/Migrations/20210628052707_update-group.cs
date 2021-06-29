using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProjectManagerAPI.Migrations
{
    public partial class updategroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_UserTypes_UserTypeID",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<Guid>(
                name: "LeaderID",
                table: "Groups",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Guid>(
                name: "UserTypeID",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_UserTypes_UserTypeID",
                table: "AspNetUsers",
                column: "UserTypeID",
                principalTable: "UserTypes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_UserTypes_UserTypeID",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LeaderID",
                table: "Groups");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserTypeID",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_UserTypes_UserTypeID",
                table: "AspNetUsers",
                column: "UserTypeID",
                principalTable: "UserTypes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
