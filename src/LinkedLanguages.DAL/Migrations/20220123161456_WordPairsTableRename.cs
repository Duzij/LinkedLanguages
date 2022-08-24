using Microsoft.EntityFrameworkCore.Migrations;

namespace LinkedLanguages.DAL.Migrations
{
    public partial class WordPairsTableRename : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WordPairToApplicationUsers_UnusedWordPairs_WordPairId",
                table: "WordPairToApplicationUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UnusedWordPairs",
                table: "UnusedWordPairs");

            migrationBuilder.RenameTable(
                name: "UnusedWordPairs",
                newName: "WordPairs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WordPairs",
                table: "WordPairs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WordPairToApplicationUsers_WordPairs_WordPairId",
                table: "WordPairToApplicationUsers",
                column: "WordPairId",
                principalTable: "WordPairs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WordPairToApplicationUsers_WordPairs_WordPairId",
                table: "WordPairToApplicationUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WordPairs",
                table: "WordPairs");

            migrationBuilder.RenameTable(
                name: "WordPairs",
                newName: "UnusedWordPairs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UnusedWordPairs",
                table: "UnusedWordPairs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WordPairToApplicationUsers_UnusedWordPairs_WordPairId",
                table: "WordPairToApplicationUsers",
                column: "WordPairId",
                principalTable: "UnusedWordPairs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
