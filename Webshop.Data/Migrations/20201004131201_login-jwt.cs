using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Webshop.Data.Migrations
{
    public partial class loginjwt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshToken_Users_UserId",
                table: "RefreshToken");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RefreshToken",
                table: "RefreshToken");

            migrationBuilder.RenameTable(
                name: "RefreshToken",
                newName: "Users_RefreshTokens");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshToken_UserId",
                table: "Users_RefreshTokens",
                newName: "IX_Users_RefreshTokens_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users_RefreshTokens",
                table: "Users_RefreshTokens",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Logins_RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(nullable: true),
                    Expires = table.Column<DateTime>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedByIp = table.Column<string>(nullable: true),
                    Revoked = table.Column<DateTime>(nullable: true),
                    RevokedByIp = table.Column<string>(nullable: true),
                    ReplacedByToken = table.Column<string>(nullable: true),
                    LoginId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logins_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Logins_RefreshTokens_Logins_LoginId",
                        column: x => x.LoginId,
                        principalTable: "Logins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Logins_RefreshTokens_LoginId",
                table: "Logins_RefreshTokens",
                column: "LoginId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_RefreshTokens_Users_UserId",
                table: "Users_RefreshTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_RefreshTokens_Users_UserId",
                table: "Users_RefreshTokens");

            migrationBuilder.DropTable(
                name: "Logins_RefreshTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users_RefreshTokens",
                table: "Users_RefreshTokens");

            migrationBuilder.RenameTable(
                name: "Users_RefreshTokens",
                newName: "RefreshToken");

            migrationBuilder.RenameIndex(
                name: "IX_Users_RefreshTokens_UserId",
                table: "RefreshToken",
                newName: "IX_RefreshToken_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RefreshToken",
                table: "RefreshToken",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshToken_Users_UserId",
                table: "RefreshToken",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
