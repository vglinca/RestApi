using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CLibrary.API.Migrations
{
    public partial class AddDateOfDeathToAuthorMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DateOdDeath",
                table: "Authors",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateOdDeath",
                table: "Authors");
        }
    }
}
