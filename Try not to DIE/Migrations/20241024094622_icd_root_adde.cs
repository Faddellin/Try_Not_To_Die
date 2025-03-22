using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Try_not_to_DIE.Migrations
{
    /// <inheritdoc />
    public partial class icd_root_adde : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Icd10_Icd10_parentid",
                table: "Icd10");

            migrationBuilder.DropIndex(
                name: "IX_Icd10_parentid",
                table: "Icd10");

            migrationBuilder.DropColumn(
                name: "parentid",
                table: "Icd10");

            migrationBuilder.CreateIndex(
                name: "IX_Icd10_rootId",
                table: "Icd10",
                column: "rootId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Icd10_Icd10_rootId",
                table: "Icd10",
                column: "rootId",
                principalTable: "Icd10",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Icd10_Icd10_rootId",
                table: "Icd10");

            migrationBuilder.DropIndex(
                name: "IX_Icd10_rootId",
                table: "Icd10");

            migrationBuilder.AddColumn<Guid>(
                name: "parentid",
                table: "Icd10",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Icd10_parentid",
                table: "Icd10",
                column: "parentid");

            migrationBuilder.AddForeignKey(
                name: "FK_Icd10_Icd10_parentid",
                table: "Icd10",
                column: "parentid",
                principalTable: "Icd10",
                principalColumn: "id");
        }
    }
}
