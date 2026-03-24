using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StratFlux.Data.Migrations
{
    public partial class RemovedIncorrectEntityRelationshipInTblIndicators : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TblIndicators_TblCharts_ChartId",
                schema: "Identity",
                table: "TblIndicators");

            migrationBuilder.DropIndex(
                name: "IX_TblIndicators_ChartId",
                schema: "Identity",
                table: "TblIndicators");

            migrationBuilder.DropColumn(
                name: "ChartId",
                schema: "Identity",
                table: "TblIndicators");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChartId",
                schema: "Identity",
                table: "TblIndicators",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TblIndicators_ChartId",
                schema: "Identity",
                table: "TblIndicators",
                column: "ChartId");

            migrationBuilder.AddForeignKey(
                name: "FK_TblIndicators_TblCharts_ChartId",
                schema: "Identity",
                table: "TblIndicators",
                column: "ChartId",
                principalSchema: "Identity",
                principalTable: "TblCharts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
