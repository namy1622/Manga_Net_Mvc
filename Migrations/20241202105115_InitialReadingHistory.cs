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
            migrationBuilder.RenameColumn(
                name: "IdChapter",
                table: "ReadingHistory",
                newName: "NameChapter");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NameChapter",
                table: "ReadingHistory",
                newName: "IdChapter");
        }
    }
}
