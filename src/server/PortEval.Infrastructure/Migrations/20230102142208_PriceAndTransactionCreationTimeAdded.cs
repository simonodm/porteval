using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace PortEval.Infrastructure.Migrations
{
    public partial class PriceAndTransactionCreationTimeAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "Transactions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "InstrumentPrices",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.Sql("UPDATE [dbo].[Transactions] SET [CreationTime] = [Time]");
            migrationBuilder.Sql("UPDATE [dbo].[InstrumentPrices] SET [CreationTime] = [Time]");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "InstrumentPrices");
        }
    }
}
