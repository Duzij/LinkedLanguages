using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkedLanguages.DAL.Migrations
{
    public partial class WordPairColumnRename : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "KnownLanguage",
                table: "WordPairs",
                newName: "KnownLanguageCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "KnownLanguageCode",
                table: "WordPairs",
                newName: "KnownLanguage");
        }
    }
}
