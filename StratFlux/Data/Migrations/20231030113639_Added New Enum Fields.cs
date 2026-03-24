using Microsoft.EntityFrameworkCore.Migrations;
using StratFlux.ModelEnums;

#nullable disable

namespace StratFlux.Data.Migrations
{
    public partial class AddedNewEnumFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<CommissionFeeType>(
                name: "CommissionFeeType",
                schema: "Identity",
                table: "TblBacktestingSettings",
                type: "\"Identity\".commission_fee_type",
                nullable: false,
                defaultValue: CommissionFeeType.Percentage);

            migrationBuilder.AddColumn<TimeResolution>(
                name: "TimeResolution",
                schema: "Identity",
                table: "TblBacktestingSettings",
                type: "\"Identity\".time_resolution",
                nullable: false,
                defaultValue: TimeResolution.PerMinute);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
