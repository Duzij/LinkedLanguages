using Microsoft.EntityFrameworkCore.Migrations;

namespace LinkedLanguages.Data.Migrations
{
    public partial class LanguageToUserAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LanguageToUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    ApplicationUserId1 = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ApplicationUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanguageToUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LanguageToUsers_AspNetUsers_ApplicationUserId1",
                        column: x => x.ApplicationUserId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LanguageToUsers_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WordPairs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WordPairs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WordPairToApplicationUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserId1 = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ApplicationUserId = table.Column<int>(type: "int", nullable: false),
                    WordPairId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WordPairToApplicationUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WordPairToApplicationUsers_AspNetUsers_ApplicationUserId1",
                        column: x => x.ApplicationUserId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WordPairToApplicationUsers_WordPairs_WordPairId",
                        column: x => x.WordPairId,
                        principalTable: "WordPairs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LanguageToUsers_ApplicationUserId1",
                table: "LanguageToUsers",
                column: "ApplicationUserId1");

            migrationBuilder.CreateIndex(
                name: "IX_LanguageToUsers_LanguageId",
                table: "LanguageToUsers",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_WordPairToApplicationUsers_ApplicationUserId1",
                table: "WordPairToApplicationUsers",
                column: "ApplicationUserId1");

            migrationBuilder.CreateIndex(
                name: "IX_WordPairToApplicationUsers_WordPairId",
                table: "WordPairToApplicationUsers",
                column: "WordPairId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LanguageToUsers");

            migrationBuilder.DropTable(
                name: "WordPairToApplicationUsers");

            migrationBuilder.DropTable(
                name: "WordPairs");
        }
    }
}
