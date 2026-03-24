using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StratFlux.Data.Migrations
{
    public partial class RenamedStockTradedFieldInGeneralResults : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StockToTrade",
                schema: "Identity",
                table: "TblGeneralResults",
                newName: "StockTraded");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StockTraded",
                schema: "Identity",
                table: "TblGeneralResults",
                newName: "StockToTrade");
        }
    }
}
