using Microsoft.EntityFrameworkCore.Migrations;
using StratFlux.ModelEnums;

#nullable disable

namespace StratFlux.Data.Migrations
{
    public partial class GeneralResultsTimeResolutionSetToEnumType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeResolution>(
                name: "TimeResolution",
                schema: "Identity",
                table: "TblGeneralResults",
                type: "\"Identity\".time_resolution",
                nullable: false,
                defaultValue: TimeResolution.PerMinute);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeResolution",
                schema: "Identity",
                table: "TblGeneralResults");
        }
    }
}
