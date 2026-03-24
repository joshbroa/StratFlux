using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StratFlux.Data.Migrations
{
    public partial class RemovedOldEnumFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommissionFeeType",
                schema: "Identity",
                table: "TblBacktestingSettings");

            migrationBuilder.DropColumn(
                name: "TimeResolution",
                schema: "Identity",
                table: "TblBacktestingSettings");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CommissionFeeType",
                schema: "Identity",
                table: "TblBacktestingSettings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TimeResolution",
                schema: "Identity",
                table: "TblBacktestingSettings",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
