using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StratFlux.Data.Migrations
{
    public partial class IntermediateChartIndicatorTablePlusOtherMinorChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SecondaryLineColour",
                schema: "Identity",
                table: "TblIndicators",
                type: "character varying(6)",
                maxLength: 6,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PrimaryLineColour",
                schema: "Identity",
                table: "TblIndicators",
                type: "character varying(6)",
                maxLength: 6,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "PositiveAreaColour",
                schema: "Identity",
                table: "TblIndicators",
                type: "character varying(6)",
                maxLength: 6,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NegativeAreaColour",
                schema: "Identity",
                table: "TblIndicators",
                type: "character varying(6)",
                maxLength: 6,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IndicatorName",
                schema: "Identity",
                table: "TblIndicators",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "TblChartIndicatorMappings",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ChartId = table.Column<string>(type: "text", nullable: false),
                    IndicatorId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblChartIndicatorMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblChartIndicatorMappings_TblCharts_ChartId",
                        column: x => x.ChartId,
                        principalSchema: "Identity",
                        principalTable: "TblCharts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblChartIndicatorMappings_TblIndicators_IndicatorId",
                        column: x => x.IndicatorId,
                        principalSchema: "Identity",
                        principalTable: "TblIndicators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TblChartIndicatorMappings_ChartId",
                schema: "Identity",
                table: "TblChartIndicatorMappings",
                column: "ChartId");

            migrationBuilder.CreateIndex(
                name: "IX_TblChartIndicatorMappings_IndicatorId",
                schema: "Identity",
                table: "TblChartIndicatorMappings",
                column: "IndicatorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TblChartIndicatorMappings",
                schema: "Identity");

            migrationBuilder.AlterColumn<string>(
                name: "SecondaryLineColour",
                schema: "Identity",
                table: "TblIndicators",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(6)",
                oldMaxLength: 6,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PrimaryLineColour",
                schema: "Identity",
                table: "TblIndicators",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(6)",
                oldMaxLength: 6);

            migrationBuilder.AlterColumn<string>(
                name: "PositiveAreaColour",
                schema: "Identity",
                table: "TblIndicators",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(6)",
                oldMaxLength: 6,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NegativeAreaColour",
                schema: "Identity",
                table: "TblIndicators",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(6)",
                oldMaxLength: 6,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IndicatorName",
                schema: "Identity",
                table: "TblIndicators",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);
        }
    }
}
