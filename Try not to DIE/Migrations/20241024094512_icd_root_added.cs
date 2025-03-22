using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Try_not_to_DIE.Migrations
{
    /// <inheritdoc />
    public partial class icd_root_added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "tempId",
                table: "Icd10");

            migrationBuilder.DropColumn(
                name: "tempPatentId",
                table: "Icd10");

            migrationBuilder.AddColumn<Guid>(
                name: "rootId",
                table: "Icd10",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "rootId",
                table: "Icd10");

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
        }
    }
}
