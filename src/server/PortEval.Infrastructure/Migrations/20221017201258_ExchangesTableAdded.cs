using Microsoft.EntityFrameworkCore.Migrations;
using System.Collections.Generic;
using System.Linq;

namespace PortEval.Infrastructure.Migrations
{
    public partial class ExchangesTableAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Exchange",
                table: "Instruments",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(32)",
                oldMaxLength: 32);

            migrationBuilder.CreateTable(
                name: "Exchanges",
                columns: table => new
                {
                    Symbol = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exchanges", x => x.Symbol);
                });

            var exchangesDataFirstBatch = new object[,]
            {
                { "AEX", "NYSE Euronext Amsterdam", 0 },
                { "RSE", "NASDAQ OMX Riga", 0 },
                { "QSE", "Qatar Stock Exchange", 0 },
                { "PSI", "NYSE Euronext Lisbon", 0 },
                { "PINK", "OTC Markets Pink Sheets", 0 },
                { "PAR", "NYSE Euronext Paris", 0 },
                { "OTC", "OTC Market", 0 },
                { "OSL", "Oslo Stock Exchange", 0 },
                { "SAO", "Sao Paolo Stock Exchange (BOVESPA)", 0 },
                { "OPRA", "Options Pricing Authority Options Chains", 0 },
                { "NZE", "New Zealand Stock Exchange", 0 },
                { "NYSE", "New York Stock Exchange", 0 },
                { "NYMEX", "New York Mercantile Exchange", 0 },
                { "NYBOT", "New York Board of Trade", 0 },
                { "NSE", "National Stock Exchange of India", 0 },
                { "NASDAQ", "NASDAQ Stock Exchange", 0 },
                { "MX", "Montreal Stock Exchange", 0 },
                { "OMXH", "NASDAQ OMX Helsinki", 0 },
                { "SEK", "NASDAQ OMX Stockholm", 0 },
                { "SET", "Thailand Stock Exchange", 0 },
                { "SGO", "Santiago Stock Exchange", 0 },
                { "WSE", "Warsaw Stock Exchange", 0 },
                { "WBAG", "Vienna Stock Exchange (Wiener Borse)", 0 },
                { "VTX", "SIX Swiss Exchange", 0 },
                { "VSE", "NASDAQ OMX Vilnius", 0 },
                { "TWO", "Taiwan OTC Exchange", 0 },
                { "TSXV", "TSX Venture Exchange", 0 },
                { "TSX", "Toronto Stock Exchange", 0 },
                { "TSE", "Tokyo Stock Exchange", 0 },
                { "TLV", "Tel Aviv Stock Exchange", 0 },
                { "TAL", "NASDAQ OMX Tallinn", 0 },
                { "TAIEX", "Taiwan Stock Exchange", 0 },
                { "TADAWUL", "Saudi Stock Exchange", 0 },
                { "SZSE", "Shenzhen Stock Exchange", 0 },
                { "STU", "Stuttgart Stock Exchange", 0 },
                { "SSE", "Shanghai Stock Exchange", 0 },
                { "SIX", "SIX Swiss Exchange", 0 },
                { "SGX", "Singapore Stock Exchange", 0 },
                { "MUN", "Munich Stock Exchange", 0 },
                { "MSE", "Madrid SE C.A.T.S.", 0 },
                { "MIL", "Borsa Italiana Milan Stock Exchange", 0 },
                { "MICEX", "Moscow Exchange", 0 }
            };

            var exchangesDataSecondBatch = new object[,]
            {
                { "CME", "Chicago Mercantile Exchange", 0 },
                { "CDNX", "Canadian Securities Exchange", 0 },
                { "CBOT", "Chicago Board of Trade", 0 },
                { "BVMF", "BM&F Bovespa", 0 },
                { "BVC", "Bolsa de Valores de Caracas", 0 },
                { "BUE", "Bolsa de Comercio de Buenos Aires", 0 },
                { "BSE", "Bombay Stock Exchange Limited", 0 },
                { "BME", "BME Spanish Exchanges", 0 },
                { "BIL", "Bilbao Stock Exchange", 0 },
                { "BER", "Berlin Stock Exchange", 0 },
                { "BEL", "NYSE Euronext Brussels", 0 },
                { "BATS", "BATS Global Markets", 0 },
                { "BAR", "Barcelona Stock Exchange", 0 },
                { "ASX", "Australian Stock Exchange", 0 },
                { "ASE", "Athens Stock Exchange", 0 },
                { "AMEX", "American Stock Exchange", 0 },
                { "ALBRTA", "Alberta Stock Exchange", 0 },
                { "CSE", "NASDAQ OMX Copenhagen", 0 },
                { "XETRA", "Deutsche Borse XETRA", 0 },
                { "DIV", "DIV", 0 },
                { "ETF", "ETF", 0 },
                { "MF", "Madrid Fixed Income Market", 0 },
                { "MEX", "Mexico Stock Exchange", 0 },
                { "MAD", "Madrid Stock Exchange", 0 },
                { "LSE", "London Stock Exchange", 0 },
                { "KSCQ", "KOSDAQ", 0 },
                { "KSC", "Korea Stock Exchange", 0 },
                { "JSE", "Johannesburg Stock Exchange", 0 },
                { "IST", "Borsa Istanbul", 0 },
                { "INDEX", "Indices", 0 },
                { "IDX", "Indonesia Stock Exchange", 0 },
                { "ICEX", "NASDAQ OMX Iceland", 0 },
                { "HKEX", "Hong Kong Stock Exchange", 0 },
                { "HAN", "Hanover Stock Exchange", 0 },
                { "HAM", "Hamburg Stock Exchange", 0 },
                { "GREY", "Grey Market", 0 },
                { "FUND", "Mutual Fund", 0 },
                { "FRA", "Deutsche B�rse Frankfurt Stock Exchange", 0 },
                { "DUS", "Dusseldorf Stock Exchange", 0 },
                { "XPRA", "Prague Stock Exchange", 0 }
            };

            migrationBuilder.InsertData(
                table: "Exchanges",
                columns: new[] { "Symbol", "Name", "Version" },
                values: exchangesDataFirstBatch);

            migrationBuilder.InsertData(
                table: "Exchanges",
                columns: new[] { "Symbol", "Name", "Version" },
                values: exchangesDataSecondBatch);

            migrationBuilder.CreateIndex(
                name: "IX_Instruments_Exchange",
                table: "Instruments",
                column: "Exchange");

            // Adjust existing instruments by removing exchanges which would violate the FK.
            var exchangeSymbols = new List<string>();
            for (int i = 0; i < exchangesDataFirstBatch.GetLength(0); i++)
            {
                exchangeSymbols.Add(exchangesDataFirstBatch[i, 0] as string);
            }
            for (int i = 0; i < exchangesDataSecondBatch.GetLength(0); i++)
            {
                exchangeSymbols.Add(exchangesDataSecondBatch[i, 0] as string);
            }

            migrationBuilder.Sql(
                $"UPDATE [dbo].[Instruments] SET [Exchange] = NULL WHERE [Exchange] NOT IN ({string.Join(",", exchangeSymbols.Select(sym => "'" + sym + "'"))})",
                true
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Instruments_Exchanges_Exchange",
                table: "Instruments",
                column: "Exchange",
                principalTable: "Exchanges",
                principalColumn: "Symbol",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE [dbo].[Instruments] SET [Exchange] = '' WHERE [Exchange] IS NULL");

            migrationBuilder.DropForeignKey(
                name: "FK_Instruments_Exchanges_Exchange",
                table: "Instruments");

            migrationBuilder.DropTable(
                name: "Exchanges");

            migrationBuilder.DropIndex(
                name: "IX_Instruments_Exchange",
                table: "Instruments");

            migrationBuilder.AlterColumn<string>(
                name: "Exchange",
                table: "Instruments",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(32)",
                oldMaxLength: 32,
                oldNullable: true);
        }
    }
}
