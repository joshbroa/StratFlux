using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StratFlux.Data.Migrations
{
    public partial class AddedEnumTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TblRolesClaims_TblRoles_RoleId",
                schema: "Identity",
                table: "TblRolesClaims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TblRolesClaims",
                schema: "Identity",
                table: "TblRolesClaims");

            migrationBuilder.RenameTable(
                name: "TblRolesClaims",
                schema: "Identity",
                newName: "TblRoleClaims",
                newSchema: "Identity");

            migrationBuilder.RenameIndex(
                name: "IX_TblRolesClaims_RoleId",
                schema: "Identity",
                table: "TblRoleClaims",
                newName: "IX_TblRoleClaims_RoleId");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:Identity.commission_fee_type", "percentage,absolute")
                .Annotation("Npgsql:Enum:Identity.time_resolution", "per_minute,hourly,daily,weekly,monthly");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TblRoleClaims",
                schema: "Identity",
                table: "TblRoleClaims",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TblRoleClaims_TblRoles_RoleId",
                schema: "Identity",
                table: "TblRoleClaims",
                column: "RoleId",
                principalSchema: "Identity",
                principalTable: "TblRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TblRoleClaims_TblRoles_RoleId",
                schema: "Identity",
                table: "TblRoleClaims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TblRoleClaims",
                schema: "Identity",
                table: "TblRoleClaims");

            migrationBuilder.RenameTable(
                name: "TblRoleClaims",
                schema: "Identity",
                newName: "TblRolesClaims",
                newSchema: "Identity");

            migrationBuilder.RenameIndex(
                name: "IX_TblRoleClaims_RoleId",
                schema: "Identity",
                table: "TblRolesClaims",
                newName: "IX_TblRolesClaims_RoleId");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:Enum:Identity.commission_fee_type", "percentage,absolute")
                .OldAnnotation("Npgsql:Enum:Identity.time_resolution", "per_minute,hourly,daily,weekly,monthly");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TblRolesClaims",
                schema: "Identity",
                table: "TblRolesClaims",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TblRolesClaims_TblRoles_RoleId",
                schema: "Identity",
                table: "TblRolesClaims",
                column: "RoleId",
                principalSchema: "Identity",
                principalTable: "TblRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
