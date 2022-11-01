using Microsoft.EntityFrameworkCore.Migrations;


namespace LinkedLanguages.DAL.Migrations
{
    public partial class StatisticsAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfFailedSubmissions",
                table: "WordPairToApplicationUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_WordPairs_KnownLanguageId",
                table: "WordPairs",
                column: "KnownLanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_WordPairs_UnknownLanguageId",
                table: "WordPairs",
                column: "UnknownLanguageId");

            migrationBuilder.AddForeignKey(
                name: "FK_WordPairs_Languages_KnownLanguageId",
                table: "WordPairs",
                column: "KnownLanguageId",
                principalTable: "Languages",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_WordPairs_Languages_UnknownLanguageId",
                table: "WordPairs",
                column: "UnknownLanguageId",
                principalTable: "Languages",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WordPairs_Languages_KnownLanguageId",
                table: "WordPairs");

            migrationBuilder.DropForeignKey(
                name: "FK_WordPairs_Languages_UnknownLanguageId",
                table: "WordPairs");

            migrationBuilder.DropIndex(
                name: "IX_WordPairs_KnownLanguageId",
                table: "WordPairs");

            migrationBuilder.DropIndex(
                name: "IX_WordPairs_UnknownLanguageId",
                table: "WordPairs");

            migrationBuilder.DropColumn(
                name: "NumberOfFailedSubmissions",
                table: "WordPairToApplicationUsers");
        }
    }
}
