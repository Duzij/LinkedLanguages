using Microsoft.EntityFrameworkCore.Migrations;

namespace LinkedLanguages.DAL.Migrations
{
    public partial class UrisAddedToWords : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "KnownWordUri",
                table: "WordPairs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnknownWordUri",
                table: "WordPairs",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KnownWordUri",
                table: "WordPairs");

            migrationBuilder.DropColumn(
                name: "UnknownWordUri",
                table: "WordPairs");
        }
    }
}
