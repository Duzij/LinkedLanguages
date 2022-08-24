using Microsoft.EntityFrameworkCore.Migrations;

namespace LinkedLanguages.DAL.Migrations
{
    public partial class WordPairToApplicationUserRedundantuserRemoved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WordPairToApplicationUsers_AspNetUsers_ApplicationUserId1",
                table: "WordPairToApplicationUsers");

            migrationBuilder.DropIndex(
                name: "IX_WordPairToApplicationUsers_ApplicationUserId1",
                table: "WordPairToApplicationUsers");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId1",
                table: "WordPairToApplicationUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId1",
                table: "WordPairToApplicationUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WordPairToApplicationUsers_ApplicationUserId1",
                table: "WordPairToApplicationUsers",
                column: "ApplicationUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_WordPairToApplicationUsers_AspNetUsers_ApplicationUserId1",
                table: "WordPairToApplicationUsers",
                column: "ApplicationUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
