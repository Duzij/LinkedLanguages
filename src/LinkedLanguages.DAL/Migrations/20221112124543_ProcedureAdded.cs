using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkedLanguages.DAL.Migrations
{
    public partial class ProcedureAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "CREATE FUNCTION UsedCount" +
                "(@WordPairId uniqueidentifier)" +
                "RETURNS int " +
                "AS " +
                "BEGIN RETURN(" +
                "SELECT TOP 1 COUNT(Id) as wordPairCount " +
                "FROM [WordPairToApplicationUsers] " +
                "where WordPairId = @WordPairId " +
                "GROUP BY [WordPairId] " +
                "Order by wordPairCount desc) " +
                "end");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("drop function UsedCount");
        }
    }
}
