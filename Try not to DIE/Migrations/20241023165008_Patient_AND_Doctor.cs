using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Try_not_to_DIE.Migrations
{
    /// <inheritdoc />
    public partial class Patient_AND_Doctor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "lastInspectionId",
                table: "Patient");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "lastInspectionId",
                table: "Patient",
                type: "uuid",
                nullable: true);
        }
    }
}
