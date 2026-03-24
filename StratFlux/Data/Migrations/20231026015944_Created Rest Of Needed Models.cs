using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StratFlux.Data.Migrations
{
    public partial class CreatedRestOfNeededModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TblFriendList",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    User1Accepted = table.Column<bool>(type: "boolean", nullable: false),
                    User2Accepted = table.Column<bool>(type: "boolean", nullable: false),
                    User1Id = table.Column<string>(type: "text", nullable: false),
                    User2Id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblFriendList", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblFriendList_TblUsers_User1Id",
                        column: x => x.User1Id,
                        principalSchema: "Identity",
                        principalTable: "TblUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblFriendList_TblUsers_User2Id",
                        column: x => x.User2Id,
                        principalSchema: "Identity",
                        principalTable: "TblUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblGeneralResults",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ResultsName = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    UnrealisedReturnLoss = table.Column<double>(type: "double precision", nullable: false),
                    NetReturnLoss = table.Column<double>(type: "double precision", nullable: false),
                    AverageReturnLoss = table.Column<double>(type: "double precision", nullable: false),
                    AverageHoldingPeriod = table.Column<TimeSpan>(type: "interval", nullable: false),
                    StandardDeviationOverTime = table.Column<double>(type: "double precision", nullable: false),
                    NetProfitLoss = table.Column<double>(type: "double precision", nullable: false),
                    InitialEquity = table.Column<double>(type: "double precision", nullable: false),
                    FinalEquity = table.Column<double>(type: "double precision", nullable: false),
                    MaxDrawDown = table.Column<double>(type: "double precision", nullable: false),
                    TotalCommissionAmount = table.Column<double>(type: "double precision", nullable: false),
                    TotalClosedTrades = table.Column<int>(type: "integer", nullable: false),
                    WinningTrades = table.Column<int>(type: "integer", nullable: false),
                    LosingTrades = table.Column<int>(type: "integer", nullable: false),
                    TimeResolution = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblGeneralResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblGeneralResults_TblUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "TblUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblStrategies",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    StrategyName = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    StrategyDescription = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    StrategyJson = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblStrategies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TblMessageHistory",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Message = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FriendsId = table.Column<string>(type: "text", nullable: true),
                    SentByUserId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblMessageHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblMessageHistory_TblFriendList_FriendsId",
                        column: x => x.FriendsId,
                        principalSchema: "Identity",
                        principalTable: "TblFriendList",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TblMessageHistory_TblUsers_SentByUserId",
                        column: x => x.SentByUserId,
                        principalSchema: "Identity",
                        principalTable: "TblUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TblTimeSeriesResults",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    GeneralResultsId = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<double>(type: "double precision", nullable: false),
                    Index = table.Column<int>(type: "integer", nullable: false),
                    NodeId = table.Column<int>(type: "integer", nullable: true)
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
                name: "IX_TblFriendList_User1Id",
                schema: "Identity",
                table: "TblFriendList",
                column: "User1Id");

            migrationBuilder.CreateIndex(
                name: "IX_TblFriendList_User2Id",
                schema: "Identity",
                table: "TblFriendList",
                column: "User2Id");

            migrationBuilder.CreateIndex(
                name: "IX_TblGeneralResults_UserId",
                schema: "Identity",
                table: "TblGeneralResults",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TblMessageHistory_FriendsId",
                schema: "Identity",
                table: "TblMessageHistory",
                column: "FriendsId");

            migrationBuilder.CreateIndex(
                name: "IX_TblMessageHistory_SentByUserId",
                schema: "Identity",
                table: "TblMessageHistory",
                column: "SentByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TblTimeSeriesResults_GeneralResultsId",
                schema: "Identity",
                table: "TblTimeSeriesResults",
                column: "GeneralResultsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TblMessageHistory",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "TblStrategies",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "TblTimeSeriesResults",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "TblFriendList",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "TblGeneralResults",
                schema: "Identity");
        }
    }
}
