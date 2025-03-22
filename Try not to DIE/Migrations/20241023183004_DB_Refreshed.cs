using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Try_not_to_DIE.Migrations
{
    /// <inheritdoc />
    public partial class DB_Refreshed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Icd10_Icd10_parentIdid",
                table: "Icd10");

            migrationBuilder.RenameColumn(
                name: "parentIdid",
                table: "Icd10",
                newName: "parentid");

            migrationBuilder.RenameIndex(
                name: "IX_Icd10_parentIdid",
                table: "Icd10",
                newName: "IX_Icd10_parentid");

            migrationBuilder.AddForeignKey(
                name: "FK_Icd10_Icd10_parentid",
                table: "Icd10",
                column: "parentid",
                principalTable: "Icd10",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Icd10_Icd10_parentid",
                table: "Icd10");

            migrationBuilder.RenameColumn(
                name: "parentid",
                table: "Icd10",
                newName: "parentIdid");

            migrationBuilder.RenameIndex(
                name: "IX_Icd10_parentid",
                table: "Icd10",
                newName: "IX_Icd10_parentIdid");

            migrationBuilder.AddForeignKey(
                name: "FK_Icd10_Icd10_parentIdid",
                table: "Icd10",
                column: "parentIdid",
                principalTable: "Icd10",
                principalColumn: "id");
        }
    }
}
