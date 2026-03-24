using System;
using Microsoft.EntityFrameworkCore.Migrations;
using StratFlux.ModelEnums;

#nullable disable

namespace StratFlux.Data.Migrations
{
    public partial class GeneralResultsFieldsNowNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "WinningTrades",
                schema: "Identity",
                table: "TblGeneralResults",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<double>(
                name: "UnrealisedReturnLoss",
                schema: "Identity",
                table: "TblGeneralResults",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<double>(
                name: "TotalCommissionAmount",
                schema: "Identity",
                table: "TblGeneralResults",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<int>(
                name: "TotalClosedTrades",
                schema: "Identity",
                table: "TblGeneralResults",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<TimeResolution>(
                name: "TimeResolution",
                schema: "Identity",
                table: "TblGeneralResults",
                type: "\"Identity\".time_resolution",
                nullable: true,
                oldClrType: typeof(TimeResolution),
                oldType: "\"Identity\".time_resolution");

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeFrameStart",
                schema: "Identity",
                table: "TblGeneralResults",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeFrameEnd",
                schema: "Identity",
                table: "TblGeneralResults",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "StockTraded",
                schema: "Identity",
                table: "TblGeneralResults",
                type: "character varying(5)",
                maxLength: 5,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(5)",
                oldMaxLength: 5);

            migrationBuilder.AlterColumn<double>(
                name: "StandardDeviationOverTime",
                schema: "Identity",
                table: "TblGeneralResults",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<string>(
                name: "ResultsName",
                schema: "Identity",
                table: "TblGeneralResults",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<double>(
                name: "NetReturnLoss",
                schema: "Identity",
                table: "TblGeneralResults",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<double>(
                name: "NetProfitLoss",
                schema: "Identity",
                table: "TblGeneralResults",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<double>(
                name: "MaxDrawDown",
                schema: "Identity",
                table: "TblGeneralResults",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<int>(
                name: "LosingTrades",
                schema: "Identity",
                table: "TblGeneralResults",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<double>(
                name: "InitialEquity",
                schema: "Identity",
                table: "TblGeneralResults",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<double>(
                name: "FinalEquity",
                schema: "Identity",
                table: "TblGeneralResults",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<double>(
                name: "AverageReturnLoss",
                schema: "Identity",
                table: "TblGeneralResults",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "AverageHoldingPeriod",
                schema: "Identity",
                table: "TblGeneralResults",
                type: "interval",
                nullable: true,
                oldClrType: typeof(TimeSpan),
                oldType: "interval");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "WinningTrades",
                schema: "Identity",
                table: "TblGeneralResults",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "UnrealisedReturnLoss",
                schema: "Identity",
                table: "TblGeneralResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "TotalCommissionAmount",
                schema: "Identity",
                table: "TblGeneralResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TotalClosedTrades",
                schema: "Identity",
                table: "TblGeneralResults",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeResolution>(
                name: "TimeResolution",
                schema: "Identity",
                table: "TblGeneralResults",
                type: "\"Identity\".time_resolution",
                nullable: false,
                defaultValue: TimeResolution.PerMinute,
                oldClrType: typeof(TimeResolution),
                oldType: "\"Identity\".time_resolution",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeFrameStart",
                schema: "Identity",
                table: "TblGeneralResults",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeFrameEnd",
                schema: "Identity",
                table: "TblGeneralResults",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StockTraded",
                schema: "Identity",
                table: "TblGeneralResults",
                type: "character varying(5)",
                maxLength: 5,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(5)",
                oldMaxLength: 5,
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "StandardDeviationOverTime",
                schema: "Identity",
                table: "TblGeneralResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ResultsName",
                schema: "Identity",
                table: "TblGeneralResults",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "NetReturnLoss",
                schema: "Identity",
                table: "TblGeneralResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "NetProfitLoss",
                schema: "Identity",
                table: "TblGeneralResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "MaxDrawDown",
                schema: "Identity",
                table: "TblGeneralResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LosingTrades",
                schema: "Identity",
                table: "TblGeneralResults",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "InitialEquity",
                schema: "Identity",
                table: "TblGeneralResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "FinalEquity",
                schema: "Identity",
                table: "TblGeneralResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "AverageReturnLoss",
                schema: "Identity",
                table: "TblGeneralResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "AverageHoldingPeriod",
                schema: "Identity",
                table: "TblGeneralResults",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0),
                oldClrType: typeof(TimeSpan),
                oldType: "interval",
                oldNullable: true);
        }
    }
}
