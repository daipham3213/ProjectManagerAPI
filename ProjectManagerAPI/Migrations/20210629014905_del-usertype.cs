using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProjectManagerAPI.Migrations
{
    public partial class delusertype : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_UserTypes_UserTypeID",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "UserTypes");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_UserTypeID",
                table: "AspNetUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserTypes",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActived = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentNID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserCreated = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTypes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserTypes_UserTypes_ParentNID",
                        column: x => x.ParentNID,
                        principalTable: "UserTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UserTypeID",
                table: "AspNetUsers",
                column: "UserTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_UserTypes_ParentNID",
                table: "UserTypes",
                column: "ParentNID");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_UserTypes_UserTypeID",
                table: "AspNetUsers",
                column: "UserTypeID",
                principalTable: "UserTypes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
