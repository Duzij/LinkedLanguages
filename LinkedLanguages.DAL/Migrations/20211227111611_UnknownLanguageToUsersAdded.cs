using Microsoft.EntityFrameworkCore.Migrations;

namespace LinkedLanguages.Data.Migrations
{
    public partial class UnknownLanguageToUsersAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LanguageToUsers");

            migrationBuilder.CreateTable(
                name: "KnownLanguageToUsers",
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
                    table.PrimaryKey("PK_KnownLanguageToUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KnownLanguageToUsers_AspNetUsers_ApplicationUserId1",
                        column: x => x.ApplicationUserId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KnownLanguageToUsers_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UnknownLanguageToUsers",
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
                    table.PrimaryKey("PK_UnknownLanguageToUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnknownLanguageToUsers_AspNetUsers_ApplicationUserId1",
                        column: x => x.ApplicationUserId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UnknownLanguageToUsers_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KnownLanguageToUsers_ApplicationUserId1",
                table: "KnownLanguageToUsers",
                column: "ApplicationUserId1");

            migrationBuilder.CreateIndex(
                name: "IX_KnownLanguageToUsers_LanguageId",
                table: "KnownLanguageToUsers",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_UnknownLanguageToUsers_ApplicationUserId1",
                table: "UnknownLanguageToUsers",
                column: "ApplicationUserId1");

            migrationBuilder.CreateIndex(
                name: "IX_UnknownLanguageToUsers_LanguageId",
                table: "UnknownLanguageToUsers",
                column: "LanguageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KnownLanguageToUsers");

            migrationBuilder.DropTable(
                name: "UnknownLanguageToUsers");

            migrationBuilder.CreateTable(
                name: "LanguageToUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserId = table.Column<int>(type: "int", nullable: false),
                    ApplicationUserId1 = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LanguageId = table.Column<int>(type: "int", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_LanguageToUsers_ApplicationUserId1",
                table: "LanguageToUsers",
                column: "ApplicationUserId1");

            migrationBuilder.CreateIndex(
                name: "IX_LanguageToUsers_LanguageId",
                table: "LanguageToUsers",
                column: "LanguageId");
        }
    }
}
