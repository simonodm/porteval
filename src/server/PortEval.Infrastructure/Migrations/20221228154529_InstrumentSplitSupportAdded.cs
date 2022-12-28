using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PortEval.Infrastructure.Migrations
{
    public partial class InstrumentSplitSupportAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "IsTracked",
                table: "Instruments");

            migrationBuilder.AddColumn<DateTime>(
                name: "TrackingInfo_TrackedSince",
                table: "Instruments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrackingStatus",
                table: "Instruments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "InstrumentPrices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Imports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "TrackingInfo_TrackedSince",
                table: "Currencies",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InstrumentSplits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InstrumentId = table.Column<int>(type: "int", nullable: false),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessingStatus = table.Column<int>(type: "int", nullable: false),
                    SplitRatioDenominator = table.Column<int>(type: "int", nullable: false),
                    SplitRatioNumerator = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstrumentSplits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InstrumentSplits_Instruments_InstrumentId",
                        column: x => x.InstrumentId,
                        principalTable: "Instruments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Exchanges",
                keyColumn: "Symbol",
                keyValue: "FRA",
                column: "Name",
                value: "Deutsche Börse Frankfurt Stock Exchange");

            migrationBuilder.CreateIndex(
                name: "IX_InstrumentSplits_InstrumentId",
                table: "InstrumentSplits",
                column: "InstrumentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InstrumentSplits");

            migrationBuilder.DropColumn(
                name: "TrackingInfo_TrackedSince",
                table: "Instruments");

            migrationBuilder.DropColumn(
                name: "TrackingStatus",
                table: "Instruments");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "InstrumentPrices");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Imports");

            migrationBuilder.DropColumn(
                name: "TrackingInfo_TrackedSince",
                table: "Currencies");

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

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
