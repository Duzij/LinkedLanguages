using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkedLanguages.DAL.Migrations
{
    public partial class RejectedCountColumnAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RejectedCount",
                table: "WordPairs",
                type: "int",
                nullable: true,
                computedColumnSql: "dbo.RejectedCount([Id])"
               );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectedCount",
                table: "WordPairs");
        }
    }
}
