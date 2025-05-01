using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mvcpets.Migrations
{
    /// <inheritdoc />
    public partial class Add5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPosts_AspNetUsers_UserId",
                table: "BlogPosts");

            migrationBuilder.DropIndex(
                name: "IX_BlogPosts_UserId",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "BlogPosts");

            migrationBuilder.RenameColumn(
                name: "BlogPostId",
                table: "BlogPosts",
                newName: "Id");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "BlogPosts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "BlogPosts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_UserId1",
                table: "BlogPosts",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPosts_AspNetUsers_UserId1",
                table: "BlogPosts",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPosts_AspNetUsers_UserId1",
                table: "BlogPosts");

            migrationBuilder.DropIndex(
                name: "IX_BlogPosts_UserId1",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "BlogPosts");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "BlogPosts",
                newName: "BlogPostId");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "BlogPosts",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_UserId",
                table: "BlogPosts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPosts_AspNetUsers_UserId",
                table: "BlogPosts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
