using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Try_not_to_DIE.Migrations
{
    /// <inheritdoc />
    public partial class icd_reload : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Icd10_Icd10_rootId",
                table: "Icd10");

            migrationBuilder.DropIndex(
                name: "IX_Icd10_rootId",
                table: "Icd10");

            migrationBuilder.AddColumn<Guid>(
                name: "parentId",
                table: "Icd10",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "tempId",
                table: "Icd10",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "tempPatentId",
                table: "Icd10",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Icd10_parentId",
                table: "Icd10",
                column: "parentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Icd10_Icd10_parentId",
                table: "Icd10",
                column: "parentId",
                principalTable: "Icd10",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Icd10_Icd10_parentId",
                table: "Icd10");

            migrationBuilder.DropIndex(
                name: "IX_Icd10_parentId",
                table: "Icd10");

            migrationBuilder.DropColumn(
                name: "parentId",
                table: "Icd10");

            migrationBuilder.DropColumn(
                name: "tempId",
                table: "Icd10");

            migrationBuilder.DropColumn(
                name: "tempPatentId",
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
    }
}
