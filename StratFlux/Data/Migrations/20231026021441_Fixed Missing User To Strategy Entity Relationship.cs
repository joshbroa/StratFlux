using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StratFlux.Data.Migrations
{
    public partial class FixedMissingUserToStrategyEntityRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                schema: "Identity",
                table: "TblStrategies",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TblStrategies_UserId",
                schema: "Identity",
                table: "TblStrategies",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TblStrategies_TblUsers_UserId",
                schema: "Identity",
                table: "TblStrategies",
                column: "UserId",
                principalSchema: "Identity",
                principalTable: "TblUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TblStrategies_TblUsers_UserId",
                schema: "Identity",
                table: "TblStrategies");

            migrationBuilder.DropIndex(
                name: "IX_TblStrategies_UserId",
                schema: "Identity",
                table: "TblStrategies");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "Identity",
                table: "TblStrategies");
        }
    }
}
