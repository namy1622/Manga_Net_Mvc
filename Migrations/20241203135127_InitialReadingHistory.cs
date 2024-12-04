using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BTL_WebManga.Migrations
{
    /// <inheritdoc />
    public partial class InitialReadingHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LinkChapter",
                table: "ReadingHistory",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LinkChapter",
                table: "ReadingHistory");
        }
    }
}
