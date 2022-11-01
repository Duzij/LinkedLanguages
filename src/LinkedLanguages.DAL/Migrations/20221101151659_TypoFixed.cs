using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkedLanguages.DAL.Migrations
{
    public partial class TypoFixed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Larned",
                table: "WordPairToApplicationUsers",
                newName: "Learned");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Learned",
                table: "WordPairToApplicationUsers",
                newName: "Larned");
        }
    }
}
