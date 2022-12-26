using Microsoft.EntityFrameworkCore.Migrations;

namespace PortEval.Infrastructure.Migrations
{
    public partial class InstrumentTrackingStatusAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTracked",
                table: "Instruments");

            migrationBuilder.AddColumn<int>(
                name: "TrackingStatus",
                table: "Instruments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Exchanges",
                keyColumn: "Symbol",
                keyValue: "FRA",
                column: "Name",
                value: "Deutsche Börse Frankfurt Stock Exchange");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrackingStatus",
                table: "Instruments");

            migrationBuilder.AddColumn<bool>(
                name: "IsTracked",
                table: "Instruments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Exchanges",
                keyColumn: "Symbol",
                keyValue: "FRA",
                column: "Name",
                value: "Deutsche B�rse Frankfurt Stock Exchange");
        }
    }
}
