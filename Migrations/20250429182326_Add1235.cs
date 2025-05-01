using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mvcpets.Migrations
{
    /// <inheritdoc />
    public partial class Add1235 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackgroundImage",
                table: "HomeCards");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BackgroundImage",
                table: "HomeCards",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
