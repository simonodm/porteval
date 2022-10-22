using Microsoft.EntityFrameworkCore.Migrations;

namespace PortEval.Infrastructure.Migrations
{
    public partial class VersioningAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Positions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Portfolios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Instruments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Currencies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Charts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Positions");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Portfolios");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Instruments");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Currencies");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Charts");
        }
    }
}
