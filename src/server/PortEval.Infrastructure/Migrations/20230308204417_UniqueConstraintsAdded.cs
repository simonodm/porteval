using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortEval.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UniqueConstraintsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // removes duplicate exchange rates, keeping the latest
            migrationBuilder.Sql(@"WITH DUPLICATE_RATES AS (
	            SELECT *, ROW_NUMBER() OVER (
		            PARTITION BY [Time], [CurrencyFromCode], [CurrencyToCode]
		            ORDER BY [Time] ASC
	            ) as row_num FROM [dbo].[CurrencyExchangeRates]
            )
            DELETE FROM DUPLICATE_RATES
            WHERE row_num > 1");

            // removes duplicate instrument prices, keeping the latest
            migrationBuilder.Sql(@"WITH DUPLICATE_PRICES AS (
	            SELECT *, ROW_NUMBER() OVER (
		            PARTITION BY [InstrumentId], [Time]
		            ORDER BY [Id] DESC
	            ) as row_num FROM [dbo].[InstrumentPrices]
            )
            DELETE FROM DUPLICATE_PRICES
            WHERE row_num > 1");

            // remove duplicate instruments in two steps - delete matching chart lines first, as there is no cascade delete on them
            migrationBuilder.Sql(@"WITH DUPLICATE_INSTRUMENTS AS (
                SELECT *, ROW_NUMBER() OVER (
                    PARTITION BY [Symbol]
                    ORDER BY [Id] DESC
                ) as row_num FROM [dbo].[Instruments]
            )
            DELETE FROM [dbo].[ChartLines]
            WHERE EXISTS (
	            SELECT * FROM DUPLICATE_INSTRUMENTS
	            WHERE [Id] = [dbo].[ChartLines].[InstrumentId]
	            AND row_num > 1
            )");
            
            // delete duplicate instruments
            migrationBuilder.Sql(@"WITH DUPLICATE_INSTRUMENTS AS (
                SELECT *, ROW_NUMBER() OVER (
                    PARTITION BY [Symbol]
                    ORDER BY [Id] DESC
                ) as row_num FROM [dbo].[Instruments]
            )
            DELETE FROM DUPLICATE_INSTRUMENTS
            WHERE row_num > 1");
            
            migrationBuilder.DropIndex(
                name: "IX_Positions_PortfolioId",
                table: "Positions");

            migrationBuilder.DropIndex(
                name: "IX_InstrumentSplits_InstrumentId",
                table: "InstrumentSplits");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_PortfolioId_InstrumentId",
                table: "Positions",
                columns: new[] { "PortfolioId", "InstrumentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InstrumentSplits_InstrumentId_Time",
                table: "InstrumentSplits",
                columns: new[] { "InstrumentId", "Time" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Instruments_Symbol",
                table: "Instruments",
                column: "Symbol",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InstrumentPrices_Id_Time",
                table: "InstrumentPrices",
                columns: new[] { "Id", "Time" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchangeRates_CurrencyFromCode_CurrencyToCode_Time",
                table: "CurrencyExchangeRates",
                columns: new[] { "CurrencyFromCode", "CurrencyToCode", "Time" },
                unique: true,
                filter: "[CurrencyFromCode] IS NOT NULL AND [CurrencyToCode] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Positions_PortfolioId_InstrumentId",
                table: "Positions");

            migrationBuilder.DropIndex(
                name: "IX_InstrumentSplits_InstrumentId_Time",
                table: "InstrumentSplits");

            migrationBuilder.DropIndex(
                name: "IX_Instruments_Symbol",
                table: "Instruments");

            migrationBuilder.DropIndex(
                name: "IX_InstrumentPrices_Id_Time",
                table: "InstrumentPrices");

            migrationBuilder.DropIndex(
                name: "IX_CurrencyExchangeRates_CurrencyFromCode_CurrencyToCode_Time",
                table: "CurrencyExchangeRates");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_PortfolioId",
                table: "Positions",
                column: "PortfolioId");

            migrationBuilder.CreateIndex(
                name: "IX_InstrumentSplits_InstrumentId",
                table: "InstrumentSplits",
                column: "InstrumentId");
        }
    }
}
