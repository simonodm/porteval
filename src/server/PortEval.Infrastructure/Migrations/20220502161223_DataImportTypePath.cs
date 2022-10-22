using Microsoft.EntityFrameworkCore.Migrations;

namespace PortEval.Infrastructure.Migrations
{
    public partial class DataImportTypePath : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ErrorLogPath",
                table: "Imports",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatusDetails",
                table: "Imports",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TemplateType",
                table: "Imports",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorLogPath",
                table: "Imports");

            migrationBuilder.DropColumn(
                name: "StatusDetails",
                table: "Imports");

            migrationBuilder.DropColumn(
                name: "TemplateType",
                table: "Imports");
        }
    }
}
