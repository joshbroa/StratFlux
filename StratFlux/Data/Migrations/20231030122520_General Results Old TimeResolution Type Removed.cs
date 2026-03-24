using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StratFlux.Data.Migrations
{
    public partial class GeneralResultsOldTimeResolutionTypeRemoved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeResolution",
                schema: "Identity",
                table: "TblGeneralResults");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TimeResolution",
                schema: "Identity",
                table: "TblGeneralResults",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
