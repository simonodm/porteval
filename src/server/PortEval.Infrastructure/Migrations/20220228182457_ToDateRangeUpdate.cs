using Microsoft.EntityFrameworkCore.Migrations;

namespace PortEval.Infrastructure.Migrations
{
    public partial class ToDateRangeUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ToDateRange",
                table: "Charts",
                newName: "ToDateRangeValue");

            migrationBuilder.AddColumn<int>(
                name: "ToDateRangeUnit",
                table: "Charts",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ToDateRangeUnit",
                table: "Charts");

            migrationBuilder.RenameColumn(
                name: "ToDateRangeValue",
                table: "Charts",
                newName: "ToDateRange");
        }
    }
}
