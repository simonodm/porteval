using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace PortEval.Infrastructure.Migrations
{
    public partial class TrackingInfoLastUpdateAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TrackingInfo_LastUpdate",
                table: "Instruments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TrackingInfo_LastUpdate",
                table: "Currencies",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrackingInfo_LastUpdate",
                table: "Instruments");

            migrationBuilder.DropColumn(
                name: "TrackingInfo_LastUpdate",
                table: "Currencies");
        }
    }
}
