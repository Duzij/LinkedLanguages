using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkedLanguages.DAL.Migrations
{
    public partial class DistanceAndTranliterationAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Distance",
                table: "WordPairs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "KnownWordTransliterated",
                table: "WordPairs",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnknownWordTransliterated",
                table: "WordPairs",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WordPairs_KnownWordTransliterated_UnknownWordTransliterated",
                table: "WordPairs",
                columns: new[] { "KnownWordTransliterated", "UnknownWordTransliterated" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WordPairs_KnownWordTransliterated_UnknownWordTransliterated",
                table: "WordPairs");

            migrationBuilder.DropColumn(
                name: "Distance",
                table: "WordPairs");

            migrationBuilder.DropColumn(
                name: "KnownWordTransliterated",
                table: "WordPairs");

            migrationBuilder.DropColumn(
                name: "UnknownWordTransliterated",
                table: "WordPairs");
        }
    }
}
