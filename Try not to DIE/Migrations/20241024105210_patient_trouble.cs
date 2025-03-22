using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Try_not_to_DIE.Migrations
{
    /// <inheritdoc />
    public partial class patient_trouble : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inspection_Patient_patientid",
                table: "Inspection");

            migrationBuilder.RenameColumn(
                name: "patientid",
                table: "Inspection",
                newName: "patientId");

            migrationBuilder.RenameIndex(
                name: "IX_Inspection_patientid",
                table: "Inspection",
                newName: "IX_Inspection_patientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Inspection_Patient_patientId",
                table: "Inspection",
                column: "patientId",
                principalTable: "Patient",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inspection_Patient_patientId",
                table: "Inspection");

            migrationBuilder.RenameColumn(
                name: "patientId",
                table: "Inspection",
                newName: "patientid");

            migrationBuilder.RenameIndex(
                name: "IX_Inspection_patientId",
                table: "Inspection",
                newName: "IX_Inspection_patientid");

            migrationBuilder.AddForeignKey(
                name: "FK_Inspection_Patient_patientid",
                table: "Inspection",
                column: "patientid",
                principalTable: "Patient",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
