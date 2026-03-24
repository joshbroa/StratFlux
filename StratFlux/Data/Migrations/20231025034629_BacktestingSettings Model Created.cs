using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StratFlux.Data.Migrations
{
    public partial class BacktestingSettingsModelCreated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TblBacktestingSettings",
                schema: "Identity",
                columns: table => new
                {
                    SettingsId = table.Column<string>(type: "text", nullable: false),
                    BacktestingSettingsName = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    StockToTrade = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    TimeResolution = table.Column<int>(type: "integer", nullable: false),
                    TimeFrameStart = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TimeFrameEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    InititalCapital = table.Column<double>(type: "double precision", nullable: false),
                    UnlimitedOrderSize = table.Column<bool>(type: "boolean", nullable: false),
                    OrderSize = table.Column<long>(type: "bigint", nullable: true),
                    PyramidingLimit = table.Column<long>(type: "bigint", nullable: true),
                    CommissionFeeType = table.Column<int>(type: "integer", nullable: false),
                    CommissionFee = table.Column<double>(type: "double precision", nullable: false),
                    ResetPosAtEoD = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblBacktestingSettings", x => x.SettingsId);
                    table.ForeignKey(
                        name: "FK_TblBacktestingSettings_TblUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "TblUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TblBacktestingSettings_UserId",
                schema: "Identity",
                table: "TblBacktestingSettings",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TblBacktestingSettings",
                schema: "Identity");
        }
    }
}
