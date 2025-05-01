using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mvcpets.Migrations
{
    /// <inheritdoc />
    public partial class Add : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPosts_AspNetUsers_UserId1",
                table: "BlogPosts");

            migrationBuilder.DropIndex(
                name: "IX_BlogPosts_UserId1",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "BlogPosts");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
