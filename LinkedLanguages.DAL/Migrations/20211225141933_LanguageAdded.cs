using Microsoft.EntityFrameworkCore.Migrations;

namespace LinkedLanguages.Data.Migrations
{
    public partial class LanguageAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[,]
                {
                    { 1, "lat", "Latin" },
                    { 2, "eng", "English" },
                    { 3, "ita", "Italian" },
                    { 4, "spa", "Castilian, Spanish" },
                    { 5, "fra", "French" },
                    { 6, "rus", "Russian" },
                    { 7, "deu", "German" },
                    { 8, "por", "Portuguese" },
                    { 9, "fin", "Finnish" },
                    { 10, "zho", "Chinese" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Languages");
        }
    }
}
