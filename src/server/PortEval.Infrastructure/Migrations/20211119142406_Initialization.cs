using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace PortEval.Infrastructure.Migrations
{
    public partial class Initialization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    TrackingInfo_StartTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Charts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: true),
                    Frequency = table.Column<int>(type: "int", nullable: true),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", nullable: true),
                    DateRangeStart = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateRangeEnd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsToDate = table.Column<bool>(type: "bit", nullable: true),
                    ToDateRange = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Charts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Charts_Currencies_CurrencyCode",
                        column: x => x.CurrencyCode,
                        principalTable: "Currencies",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CurrencyExchangeRates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    CurrencyFromCode = table.Column<string>(type: "nvarchar(3)", nullable: true),
                    CurrencyToCode = table.Column<string>(type: "nvarchar(3)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyExchangeRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CurrencyExchangeRates_Currencies_CurrencyFromCode",
                        column: x => x.CurrencyFromCode,
                        principalTable: "Currencies",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CurrencyExchangeRates_Currencies_CurrencyToCode",
                        column: x => x.CurrencyToCode,
                        principalTable: "Currencies",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Instruments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Exchange = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", nullable: true),
                    TrackingInfo_StartTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instruments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Instruments_Currencies_CurrencyCode",
                        column: x => x.CurrencyCode,
                        principalTable: "Currencies",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Portfolios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Portfolios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Portfolios_Currencies_CurrencyCode",
                        column: x => x.CurrencyCode,
                        principalTable: "Currencies",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DashboardItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DashboardPositionX = table.Column<int>(type: "int", nullable: true),
                    DashboardPositionY = table.Column<int>(type: "int", nullable: true),
                    DashboardWidth = table.Column<int>(type: "int", nullable: true),
                    DashboardHeight = table.Column<int>(type: "int", nullable: true),
                    Item_Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChartId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DashboardItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DashboardItems_Charts_ChartId",
                        column: x => x.ChartId,
                        principalTable: "Charts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InstrumentPrices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    InstrumentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstrumentPrices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InstrumentPrices_Instruments_InstrumentId",
                        column: x => x.InstrumentId,
                        principalTable: "Instruments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PortfolioId = table.Column<int>(type: "int", nullable: false),
                    InstrumentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Positions_Instruments_InstrumentId",
                        column: x => x.InstrumentId,
                        principalTable: "Instruments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Positions_Portfolios_PortfolioId",
                        column: x => x.PortfolioId,
                        principalTable: "Portfolios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChartLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Width = table.Column<int>(type: "int", nullable: false),
                    Dash = table.Column<int>(type: "int", nullable: false),
                    ChartId = table.Column<int>(type: "int", nullable: false),
                    Color = table.Column<int>(type: "int", nullable: false),
                    Line_Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InstrumentId = table.Column<int>(type: "int", nullable: true),
                    PortfolioId = table.Column<int>(type: "int", nullable: true),
                    PositionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChartLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChartLines_Charts_ChartId",
                        column: x => x.ChartId,
                        principalTable: "Charts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChartLines_Instruments_InstrumentId",
                        column: x => x.InstrumentId,
                        principalTable: "Instruments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChartLines_Portfolios_PortfolioId",
                        column: x => x.PortfolioId,
                        principalTable: "Portfolios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChartLines_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(19,10)", precision: 19, scale: 10, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PositionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Code", "IsDefault", "Name", "Symbol" },
                values: new object[,]
                {
                    { "AED", false, "UAE dirham", "د.إ;" },
                    { "NAD", false, "Namibian dollar", "N$" },
                    { "NGN", false, "Nigerian naira", "₦" },
                    { "NIO", false, "Nicaraguan córdoba", "C$" },
                    { "NOK", false, "Norwegian krone", "kr" },
                    { "NPR", false, "Nepalese rupee", "NRs" },
                    { "NZD", false, "New Zealand dollar", "NZ$" },
                    { "OMR", false, "Omani rial", "OMR" },
                    { "PAB", false, "Panamanian balboa", "B/." },
                    { "PEN", false, "Peruvian nuevo sol", "S/." },
                    { "PGK", false, "Papua New Guinean kina", "K" },
                    { "PHP", false, "Philippine peso", "₱" },
                    { "PKR", false, "Pakistani rupee", "Rs." },
                    { "PLN", false, "Polish zloty", "zł" },
                    { "PYG", false, "Paraguayan guarani", "₲" },
                    { "QAR", false, "Qatari riyal", "QR" },
                    { "MZM", false, "Mozambican metical", "MTn" },
                    { "MYR", false, "Malaysian ringgit", "RM" },
                    { "MXN", false, "Mexican peso", "$" },
                    { "MWK", false, "Malawian kwacha", "MK" },
                    { "LKR", false, "Sri Lankan rupee", "Rs" },
                    { "LRD", false, "Liberian dollar", "L$" },
                    { "LSL", false, "Lesotho loti", "M" },
                    { "LTL", false, "Lithuanian litas", "Lt" },
                    { "LVL", false, "Latvian lats", "Ls" },
                    { "LYD", false, "Libyan dinar", "LD" },
                    { "MAD", false, "Moroccan dirham", "MAD" },
                    { "RON", false, "Romanian leu", "L" },
                    { "MDL", false, "Moldovan leu", "MDL" },
                    { "MKD", false, "Macedonian denar", "MKD" },
                    { "MMK", false, "Myanma kyat", "K" },
                    { "MNT", false, "Mongolian tugrik", "₮" },
                    { "MOP", false, "Macanese pataca", "P" },
                    { "MRO", false, "Mauritanian ouguiya", "UM" },
                    { "MUR", false, "Mauritian rupee", "Rs" },
                    { "MVR", false, "Maldivian rufiyaa", "Rf" },
                    { "MGA", false, "Malagasy ariary", "FMG" },
                    { "LBP", false, "Lebanese lira", "£" },
                    { "RSD", false, "Serbian dinar", "din." },
                    { "SAR", false, "Saudi riyal", "SR" },
                    { "UGX", false, "Ugandan shilling", "USh" },
                    { "USD", true, "United States dollar", "US$" }
                });

            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Code", "IsDefault", "Name", "Symbol" },
                values: new object[,]
                {
                    { "UYU", false, "Uruguayan peso", "$U" },
                    { "UZS", false, "Uzbekistani som", "UZS" },
                    { "VEB", false, "Venezuelan bolivar", "Bs" },
                    { "VND", false, "Vietnamese dong", "₫" },
                    { "VUV", false, "Vanuatu vatu", "VT" },
                    { "WST", false, "Samoan tala", "WS$" },
                    { "XAF", false, "Central African CFA franc", "CFA" },
                    { "XCD", false, "East Caribbean dollar", "EC$" },
                    { "XDR", false, "Special Drawing Rights", "SDR" },
                    { "XOF", false, "West African CFA franc", "CFA" },
                    { "XPF", false, "CFP franc", "F" },
                    { "YER", false, "Yemeni rial", "YER" },
                    { "ZAR", false, "South African rand", "R" },
                    { "UAH", false, "Ukrainian hryvnia", "UAH" },
                    { "TZS", false, "Tanzanian shilling", "TZS" },
                    { "TWD", false, "New Taiwan dollar", "NT$" },
                    { "TTD", false, "Trinidad and Tobago dollar", "TT$" },
                    { "SBD", false, "Solomon Islands dollar", "SI$" },
                    { "SCR", false, "Seychellois rupee", "SR" },
                    { "SDG", false, "Sudanese pound", "SDG" },
                    { "SEK", false, "Swedish krona", "kr" },
                    { "SGD", false, "Singapore dollar", "S$" },
                    { "SHP", false, "Saint Helena pound", "£" },
                    { "SLL", false, "Sierra Leonean leone", "Le" },
                    { "RUB", false, "Russian ruble", "R" },
                    { "SOS", false, "Somali shilling", "Sh." },
                    { "SYP", false, "Syrian pound", "LS" },
                    { "SZL", false, "Swazi lilangeni", "E" },
                    { "THB", false, "Thai baht", "฿" },
                    { "TJS", false, "Tajikistani somoni", "TJS" },
                    { "TMT", false, "Turkmen manat", "m" },
                    { "TND", false, "Tunisian dinar", "DT" },
                    { "TRY", false, "Turkish new lira", "TRY" },
                    { "SRD", false, "Surinamese dollar", "$" },
                    { "LAK", false, "Lao kip", "KN" },
                    { "KZT", false, "Kazakhstani tenge", "T" },
                    { "KYD", false, "Cayman Islands dollar", "KY$" },
                    { "BTN", false, "Bhutanese ngultrum", "Nu." },
                    { "BWP", false, "Botswana pula", "P" },
                    { "BYR", false, "Belarusian ruble", "Br" },
                    { "BZD", false, "Belize dollar", "BZ$" },
                    { "CAD", false, "Canadian dollar", "$" }
                });

            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Code", "IsDefault", "Name", "Symbol" },
                values: new object[,]
                {
                    { "CDF", false, "Congolese franc", "F" },
                    { "CHF", false, "Swiss franc", "Fr." },
                    { "CLP", false, "Chilean peso", "$" },
                    { "CNY", false, "Chinese/Yuan renminbi", "¥" },
                    { "COP", false, "Colombian peso", "Col$" },
                    { "CRC", false, "Costa Rican colon", "₡" },
                    { "CUC", false, "Cuban peso", "$" },
                    { "CVE", false, "Cape Verdean escudo", "Esc" },
                    { "CZK", false, "Czech koruna", "Kč" },
                    { "DJF", false, "Djiboutian franc", "Fdj" },
                    { "BSD", false, "Bahamian dollar", "B$" },
                    { "BRL", false, "Brazilian real", "R$" },
                    { "BOB", false, "Bolivian boliviano", "Bs." },
                    { "BND", false, "Brunei dollar", "B$" },
                    { "AFN", false, "Afghan afghani", "Afs" },
                    { "ALL", false, "Albanian lek", "L" },
                    { "AMD", false, "Armenian dram", "AMD" },
                    { "ANG", false, "Netherlands Antillean gulden", "NAƒ" },
                    { "AOA", false, "Angolan kwanza", "Kz" },
                    { "ARS", false, "Argentine peso", "$" },
                    { "AUD", false, "Australian dollar", "$" },
                    { "DKK", false, "Danish krone", "Kr" },
                    { "AWG", false, "Aruban florin", "ƒ" },
                    { "BAM", false, "Bosnia and Herzegovina konvertibilna marka", "KM" },
                    { "BBD", false, "Barbadian dollar", "Bds$" },
                    { "BDT", false, "Bangladeshi taka", "৳" },
                    { "BGN", false, "Bulgarian lev", "BGN" },
                    { "BHD", false, "Bahraini dinar", ".د.ب" },
                    { "BIF", false, "Burundi franc", "FBu" },
                    { "BMD", false, "Bermudian dollar", "BD$" },
                    { "AZN", false, "Azerbaijani manat", "AZN" },
                    { "DOP", false, "Dominican peso", "RD$" },
                    { "DZD", false, "Algerian dinar", "د.ج" },
                    { "EEK", false, "Estonian kroon", "KR" },
                    { "IDR", false, "Indonesian rupiah", "Rp" },
                    { "ILS", false, "Israeli new sheqel", "₪" },
                    { "INR", false, "Indian rupee", "₹" },
                    { "IQD", false, "Iraqi dinar", "د.ع" },
                    { "IRR", false, "Iranian rial", "IRR" },
                    { "ISK", false, "Icelandic króna", "kr" },
                    { "JMD", false, "Jamaican dollar", "J$" },
                    { "HUF", false, "Hungarian forint", "Ft" }
                });

            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Code", "IsDefault", "Name", "Symbol" },
                values: new object[,]
                {
                    { "JOD", false, "Jordanian dinar", "JOD" },
                    { "KES", false, "Kenyan shilling", "KSh" },
                    { "KGS", false, "Kyrgyzstani som", "сом" },
                    { "KHR", false, "Cambodian riel", "៛" },
                    { "KMF", false, "Comorian franc", "KMF" },
                    { "KPW", false, "North Korean won", "W" },
                    { "KRW", false, "South Korean won", "W" },
                    { "KWD", false, "Kuwaiti dinar", "KWD" },
                    { "JPY", false, "Japanese yen", "¥" },
                    { "ZMK", false, "Zambian kwacha", "ZK" },
                    { "HTG", false, "Haitian gourde", "G" },
                    { "HNL", false, "Honduran lempira", "L" },
                    { "EGP", false, "Egyptian pound", "£" },
                    { "ERN", false, "Eritrean nakfa", "Nfa" },
                    { "ETB", false, "Ethiopian birr", "Br" },
                    { "EUR", false, "European Euro", "€" },
                    { "FJD", false, "Fijian dollar", "FJ$" },
                    { "FKP", false, "Falkland Islands pound", "£" },
                    { "GBP", false, "British pound", "£" },
                    { "HRK", false, "Croatian kuna", "kn" },
                    { "GEL", false, "Georgian lari", "GEL" },
                    { "GIP", false, "Gibraltar pound", "£" },
                    { "GMD", false, "Gambian dalasi", "D" },
                    { "GNF", false, "Guinean franc", "FG" },
                    { "GQE", false, "Central African CFA franc", "CFA" },
                    { "GTQ", false, "Guatemalan quetzal", "Q" },
                    { "GYD", false, "Guyanese dollar", "GY$" },
                    { "HKD", false, "Hong Kong dollar", "HK$" },
                    { "GHS", false, "Ghanaian cedi", "GH₵" },
                    { "ZWR", false, "Zimbabwean dollar", "Z$" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChartLines_ChartId",
                table: "ChartLines",
                column: "ChartId");

            migrationBuilder.CreateIndex(
                name: "IX_ChartLines_InstrumentId",
                table: "ChartLines",
                column: "InstrumentId");

            migrationBuilder.CreateIndex(
                name: "IX_ChartLines_PortfolioId",
                table: "ChartLines",
                column: "PortfolioId");

            migrationBuilder.CreateIndex(
                name: "IX_ChartLines_PositionId",
                table: "ChartLines",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Charts_CurrencyCode",
                table: "Charts",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchangeRates_CurrencyFromCode_Time",
                table: "CurrencyExchangeRates",
                columns: new[] { "CurrencyFromCode", "Time" });

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchangeRates_CurrencyToCode",
                table: "CurrencyExchangeRates",
                column: "CurrencyToCode");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardItems_ChartId",
                table: "DashboardItems",
                column: "ChartId");

            migrationBuilder.CreateIndex(
                name: "IX_InstrumentPrices_InstrumentId_Time",
                table: "InstrumentPrices",
                columns: new[] { "InstrumentId", "Time" });

            migrationBuilder.CreateIndex(
                name: "IX_Instruments_CurrencyCode",
                table: "Instruments",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_Portfolios_CurrencyCode",
                table: "Portfolios",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_InstrumentId",
                table: "Positions",
                column: "InstrumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_PortfolioId",
                table: "Positions",
                column: "PortfolioId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_PositionId_Time",
                table: "Transactions",
                columns: new[] { "PositionId", "Time" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChartLines");

            migrationBuilder.DropTable(
                name: "CurrencyExchangeRates");

            migrationBuilder.DropTable(
                name: "DashboardItems");

            migrationBuilder.DropTable(
                name: "InstrumentPrices");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Charts");

            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DropTable(
                name: "Instruments");

            migrationBuilder.DropTable(
                name: "Portfolios");

            migrationBuilder.DropTable(
                name: "Currencies");
        }
    }
}
