using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LinkedLanguages.DAL.Migrations
{
    public partial class WordPairRejectedAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WordPairToApplicationUsers_WordPairs_WordPairId",
                table: "WordPairToApplicationUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WordPairs",
                table: "WordPairs");

            migrationBuilder.RenameTable(
                name: "WordPairs",
                newName: "UnusedWordPairs");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "UnusedWordPairs",
                newName: "UnknownWord");

            migrationBuilder.AddColumn<bool>(
                name: "Rejected",
                table: "WordPairToApplicationUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "KnownLanguage",
                table: "UnusedWordPairs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "KnownLanguageId",
                table: "UnusedWordPairs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "KnownWord",
                table: "UnusedWordPairs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnknownLanguageCode",
                table: "UnusedWordPairs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UnknownLanguageId",
                table: "UnusedWordPairs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_UnusedWordPairs",
                table: "UnusedWordPairs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WordPairToApplicationUsers_UnusedWordPairs_WordPairId",
                table: "WordPairToApplicationUsers",
                column: "WordPairId",
                principalTable: "UnusedWordPairs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WordPairToApplicationUsers_UnusedWordPairs_WordPairId",
                table: "WordPairToApplicationUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UnusedWordPairs",
                table: "UnusedWordPairs");

            migrationBuilder.DropColumn(
                name: "Rejected",
                table: "WordPairToApplicationUsers");

            migrationBuilder.DropColumn(
                name: "KnownLanguage",
                table: "UnusedWordPairs");

            migrationBuilder.DropColumn(
                name: "KnownLanguageId",
                table: "UnusedWordPairs");

            migrationBuilder.DropColumn(
                name: "KnownWord",
                table: "UnusedWordPairs");

            migrationBuilder.DropColumn(
                name: "UnknownLanguageCode",
                table: "UnusedWordPairs");

            migrationBuilder.DropColumn(
                name: "UnknownLanguageId",
                table: "UnusedWordPairs");

            migrationBuilder.RenameTable(
                name: "UnusedWordPairs",
                newName: "WordPairs");

            migrationBuilder.RenameColumn(
                name: "UnknownWord",
                table: "WordPairs",
                newName: "Content");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WordPairs",
                table: "WordPairs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WordPairToApplicationUsers_WordPairs_WordPairId",
                table: "WordPairToApplicationUsers",
                column: "WordPairId",
                principalTable: "WordPairs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
