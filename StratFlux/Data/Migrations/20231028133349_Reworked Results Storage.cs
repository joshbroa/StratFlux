using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StratFlux.Data.Migrations
{
    public partial class ReworkedResultsStorage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TblMessageHistory_TblFriendList_FriendsId",
                schema: "Identity",
                table: "TblMessageHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_TblMessageHistory_TblUsers_SentByUserId",
                schema: "Identity",
                table: "TblMessageHistory");

            migrationBuilder.DropTable(
                name: "TblTimeSeriesResults",
                schema: "Identity");

            migrationBuilder.AlterColumn<string>(
                name: "SentByUserId",
                schema: "Identity",
                table: "TblMessageHistory",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FriendsId",
                schema: "Identity",
                table: "TblMessageHistory",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ReadByOtherUser",
                schema: "Identity",
                table: "TblMessageHistory",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "StockToTrade",
                schema: "Identity",
                table: "TblGeneralResults",
                type: "character varying(5)",
                maxLength: 5,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeFrameEnd",
                schema: "Identity",
                table: "TblGeneralResults",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeFrameStart",
                schema: "Identity",
                table: "TblGeneralResults",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "TblCharts",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ContainsStockData = table.Column<bool>(type: "boolean", nullable: false),
                    GeneralResultsId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblCharts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblCharts_TblGeneralResults_GeneralResultsId",
                        column: x => x.GeneralResultsId,
                        principalSchema: "Identity",
                        principalTable: "TblGeneralResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblIndicators",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    IndicatorName = table.Column<string>(type: "text", nullable: false),
                    HasArea = table.Column<bool>(type: "boolean", nullable: false),
                    PrimaryLineColour = table.Column<string>(type: "text", nullable: false),
                    SecondaryLineColour = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    PositiveAreaColour = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    NegativeAreaColour = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ChartId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblIndicators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblIndicators_TblCharts_ChartId",
                        column: x => x.ChartId,
                        principalSchema: "Identity",
                        principalTable: "TblCharts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblTimeSeriesData",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Value = table.Column<double>(type: "double precision", nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    IndicatorId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblTimeSeriesData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblTimeSeriesData_TblIndicators_IndicatorId",
                        column: x => x.IndicatorId,
                        principalSchema: "Identity",
                        principalTable: "TblIndicators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TblCharts_GeneralResultsId",
                schema: "Identity",
                table: "TblCharts",
                column: "GeneralResultsId");

            migrationBuilder.CreateIndex(
                name: "IX_TblIndicators_ChartId",
                schema: "Identity",
                table: "TblIndicators",
                column: "ChartId");

            migrationBuilder.CreateIndex(
                name: "IX_TblTimeSeriesData_IndicatorId",
                schema: "Identity",
                table: "TblTimeSeriesData",
                column: "IndicatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_TblMessageHistory_TblFriendList_FriendsId",
                schema: "Identity",
                table: "TblMessageHistory",
                column: "FriendsId",
                principalSchema: "Identity",
                principalTable: "TblFriendList",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TblMessageHistory_TblUsers_SentByUserId",
                schema: "Identity",
                table: "TblMessageHistory",
                column: "SentByUserId",
                principalSchema: "Identity",
                principalTable: "TblUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TblMessageHistory_TblFriendList_FriendsId",
                schema: "Identity",
                table: "TblMessageHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_TblMessageHistory_TblUsers_SentByUserId",
                schema: "Identity",
                table: "TblMessageHistory");

            migrationBuilder.DropTable(
                name: "TblTimeSeriesData",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "TblIndicators",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "TblCharts",
                schema: "Identity");

            migrationBuilder.DropColumn(
                name: "ReadByOtherUser",
                schema: "Identity",
                table: "TblMessageHistory");

            migrationBuilder.DropColumn(
                name: "StockToTrade",
                schema: "Identity",
                table: "TblGeneralResults");

            migrationBuilder.DropColumn(
                name: "TimeFrameEnd",
                schema: "Identity",
                table: "TblGeneralResults");

            migrationBuilder.DropColumn(
                name: "TimeFrameStart",
                schema: "Identity",
                table: "TblGeneralResults");

            migrationBuilder.AlterColumn<string>(
                name: "SentByUserId",
                schema: "Identity",
                table: "TblMessageHistory",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "FriendsId",
                schema: "Identity",
                table: "TblMessageHistory",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "TblTimeSeriesResults",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    GeneralResultsId = table.Column<string>(type: "text", nullable: false),
                    Index = table.Column<int>(type: "integer", nullable: false),
                    NodeId = table.Column<int>(type: "integer", nullable: true),
                    Value = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblTimeSeriesResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblTimeSeriesResults_TblGeneralResults_GeneralResultsId",
                        column: x => x.GeneralResultsId,
                        principalSchema: "Identity",
                        principalTable: "TblGeneralResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TblTimeSeriesResults_GeneralResultsId",
                schema: "Identity",
                table: "TblTimeSeriesResults",
                column: "GeneralResultsId");

            migrationBuilder.AddForeignKey(
                name: "FK_TblMessageHistory_TblFriendList_FriendsId",
                schema: "Identity",
                table: "TblMessageHistory",
                column: "FriendsId",
                principalSchema: "Identity",
                principalTable: "TblFriendList",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TblMessageHistory_TblUsers_SentByUserId",
                schema: "Identity",
                table: "TblMessageHistory",
                column: "SentByUserId",
                principalSchema: "Identity",
                principalTable: "TblUsers",
                principalColumn: "Id");
        }
    }
}
