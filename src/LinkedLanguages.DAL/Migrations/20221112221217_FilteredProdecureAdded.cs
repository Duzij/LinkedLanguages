using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkedLanguages.DAL.Migrations
{
    public partial class FilteredProdecureAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
              "CREATE FUNCTION UsedCountFiltered" +
              "(@WordPairId uniqueidentifier)" +
              "RETURNS int " +
              "AS " +
              "BEGIN RETURN(" +
              "SELECT TOP 1 COUNT(Id) as wordPairCount " +
              "FROM [WordPairToApplicationUsers] " +
              "where WordPairId = @WordPairId and Learned = 1 and not [NumberOfFailedSubmissions] = -1" +
              "GROUP BY [WordPairId] " +
              "Order by wordPairCount desc) " +
              "end");

            migrationBuilder.AlterColumn<int>(
                name: "UsedCount",
                table: "WordPairs",
                type: "int",
                nullable: true,
                computedColumnSql: "dbo.UsedCountFiltered([Id])",
                oldClrType: typeof(int),
                oldType: "int",
                oldComputedColumnSql: "dbo.UsedCount([Id])");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("drop function UsedCountFiltered");

            migrationBuilder.AlterColumn<int>(
                name: "UsedCount",
                table: "WordPairs",
                type: "int",
                nullable: false,
                computedColumnSql: "dbo.UsedCountFiltered([Id])",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComputedColumnSql: "dbo.UsedCount([Id])");
        }
    }
}
