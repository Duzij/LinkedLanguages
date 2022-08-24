using Microsoft.EntityFrameworkCore.Migrations;

namespace LinkedLanguages.DAL.Migrations
{
    public partial class LanguagesEntityFixes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KnownLanguageToUsers_AspNetUsers_ApplicationUserId1",
                table: "KnownLanguageToUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_UnknownLanguageToUsers_AspNetUsers_ApplicationUserId1",
                table: "UnknownLanguageToUsers");

            migrationBuilder.DropIndex(
                name: "IX_UnknownLanguageToUsers_ApplicationUserId1",
                table: "UnknownLanguageToUsers");

            migrationBuilder.DropIndex(
                name: "IX_KnownLanguageToUsers_ApplicationUserId1",
                table: "KnownLanguageToUsers");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId1",
                table: "UnknownLanguageToUsers");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId1",
                table: "KnownLanguageToUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId1",
                table: "UnknownLanguageToUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId1",
                table: "KnownLanguageToUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnknownLanguageToUsers_ApplicationUserId1",
                table: "UnknownLanguageToUsers",
                column: "ApplicationUserId1");

            migrationBuilder.CreateIndex(
                name: "IX_KnownLanguageToUsers_ApplicationUserId1",
                table: "KnownLanguageToUsers",
                column: "ApplicationUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_KnownLanguageToUsers_AspNetUsers_ApplicationUserId1",
                table: "KnownLanguageToUsers",
                column: "ApplicationUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UnknownLanguageToUsers_AspNetUsers_ApplicationUserId1",
                table: "UnknownLanguageToUsers",
                column: "ApplicationUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
