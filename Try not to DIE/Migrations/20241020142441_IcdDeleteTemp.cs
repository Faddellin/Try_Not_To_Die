using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Try_not_to_DIE.Migrations
{
    /// <inheritdoc />
    public partial class IcdDeleteTemp : Migration
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
