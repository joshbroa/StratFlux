using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StratFlux.Data.Migrations
{
    public partial class AdjustedOrderSizeSystem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnlimitedOrderSize",
                schema: "Identity",
                table: "TblBacktestingSettings");

            migrationBuilder.AlterColumn<long>(
                name: "OrderSize",
                schema: "Identity",
                table: "TblBacktestingSettings",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "OrderSize",
                schema: "Identity",
                table: "TblBacktestingSettings",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<bool>(
                name: "UnlimitedOrderSize",
                schema: "Identity",
                table: "TblBacktestingSettings",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
