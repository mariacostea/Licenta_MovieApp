using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MobyLabWebProgramming.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserMovieNavigationProps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Friendship",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Friendship_UserId",
                table: "Friendship",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendship_User_UserId",
                table: "Friendship",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friendship_User_UserId",
                table: "Friendship");

            migrationBuilder.DropIndex(
                name: "IX_Friendship_UserId",
                table: "Friendship");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Friendship");
        }
    }
}
