using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameFlow.Migrations
{
    /// <inheritdoc />
    public partial class Add : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccessTokens_UserAccesses_Sub",
                schema: "GameFlow",
                table: "AccessTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_AccessTokens_UsersData_Aud",
                schema: "GameFlow",
                table: "AccessTokens");

            migrationBuilder.DropIndex(
                name: "IX_AccessTokens_Aud",
                schema: "GameFlow",
                table: "AccessTokens");

            migrationBuilder.DropIndex(
                name: "IX_AccessTokens_Sub",
                schema: "GameFlow",
                table: "AccessTokens");

            migrationBuilder.AddColumn<Guid>(
                name: "UserAccessId",
                schema: "GameFlow",
                table: "AccessTokens",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                schema: "GameFlow",
                table: "AccessTokens",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_AccessTokens_UserAccessId",
                schema: "GameFlow",
                table: "AccessTokens",
                column: "UserAccessId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessTokens_UserId",
                schema: "GameFlow",
                table: "AccessTokens",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccessTokens_UserAccesses_UserAccessId",
                schema: "GameFlow",
                table: "AccessTokens",
                column: "UserAccessId",
                principalSchema: "GameFlow",
                principalTable: "UserAccesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccessTokens_UsersData_UserId",
                schema: "GameFlow",
                table: "AccessTokens",
                column: "UserId",
                principalSchema: "GameFlow",
                principalTable: "UsersData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccessTokens_UserAccesses_UserAccessId",
                schema: "GameFlow",
                table: "AccessTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_AccessTokens_UsersData_UserId",
                schema: "GameFlow",
                table: "AccessTokens");

            migrationBuilder.DropIndex(
                name: "IX_AccessTokens_UserAccessId",
                schema: "GameFlow",
                table: "AccessTokens");

            migrationBuilder.DropIndex(
                name: "IX_AccessTokens_UserId",
                schema: "GameFlow",
                table: "AccessTokens");

            migrationBuilder.DropColumn(
                name: "UserAccessId",
                schema: "GameFlow",
                table: "AccessTokens");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "GameFlow",
                table: "AccessTokens");

            migrationBuilder.CreateIndex(
                name: "IX_AccessTokens_Aud",
                schema: "GameFlow",
                table: "AccessTokens",
                column: "Aud");

            migrationBuilder.CreateIndex(
                name: "IX_AccessTokens_Sub",
                schema: "GameFlow",
                table: "AccessTokens",
                column: "Sub");

            migrationBuilder.AddForeignKey(
                name: "FK_AccessTokens_UserAccesses_Sub",
                schema: "GameFlow",
                table: "AccessTokens",
                column: "Sub",
                principalSchema: "GameFlow",
                principalTable: "UserAccesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccessTokens_UsersData_Aud",
                schema: "GameFlow",
                table: "AccessTokens",
                column: "Aud",
                principalSchema: "GameFlow",
                principalTable: "UsersData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
