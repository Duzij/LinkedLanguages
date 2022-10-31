using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkedLanguages.DAL.Migrations
{
    public partial class SeeAlsoRemoved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KnownSeeAlsoLink",
                table: "WordPairs");

            migrationBuilder.DropColumn(
                name: "UnknownSeeAlsoLink",
                table: "WordPairs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "KnownSeeAlsoLink",
                table: "WordPairs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnknownSeeAlsoLink",
                table: "WordPairs",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
