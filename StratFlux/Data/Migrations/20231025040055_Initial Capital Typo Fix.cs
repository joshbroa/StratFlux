using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StratFlux.Data.Migrations
{
    public partial class InitialCapitalTypoFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InititalCapital",
                schema: "Identity",
                table: "TblBacktestingSettings",
                newName: "InitialCapital");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InitialCapital",
                schema: "Identity",
                table: "TblBacktestingSettings",
                newName: "InititalCapital");
        }
    }
}
