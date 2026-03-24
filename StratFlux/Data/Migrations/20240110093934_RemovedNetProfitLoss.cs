using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StratFlux.Data.Migrations
{
    public partial class RemovedNetProfitLoss : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NetProfitLoss",
                schema: "Identity",
                table: "TblGeneralResults");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "NetProfitLoss",
                schema: "Identity",
                table: "TblGeneralResults",
                type: "double precision",
                nullable: true);
        }
    }
}
