using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Try_not_to_DIE.Migrations
{
    /// <inheritdoc />
    public partial class changedbd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InspectionConsultationModel");

            migrationBuilder.DropTable(
                name: "InspectionCommentModel");

            migrationBuilder.AddColumn<Guid>(
                name: "InspectionModelid",
                table: "Consultation",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Consultation_InspectionModelid",
                table: "Consultation",
                column: "InspectionModelid");

            migrationBuilder.AddForeignKey(
                name: "FK_Consultation_Inspection_InspectionModelid",
                table: "Consultation",
                column: "InspectionModelid",
                principalTable: "Inspection",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Consultation_Inspection_InspectionModelid",
                table: "Consultation");

            migrationBuilder.DropIndex(
                name: "IX_Consultation_InspectionModelid",
                table: "Consultation");

            migrationBuilder.DropColumn(
                name: "InspectionModelid",
                table: "Consultation");

            migrationBuilder.CreateTable(
                name: "InspectionCommentModel",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    authorid = table.Column<Guid>(type: "uuid", nullable: false),
                    content = table.Column<string>(type: "text", nullable: true),
                    createTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modifyTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    parentId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionCommentModel", x => x.id);
                    table.ForeignKey(
                        name: "FK_InspectionCommentModel_Doctor_authorid",
                        column: x => x.authorid,
                        principalTable: "Doctor",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InspectionConsultationModel",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    rootCommentid = table.Column<Guid>(type: "uuid", nullable: false),
                    specialityid = table.Column<Guid>(type: "uuid", nullable: false),
                    InspectionModelid = table.Column<Guid>(type: "uuid", nullable: true),
                    commentsNumber = table.Column<int>(type: "integer", nullable: false),
                    createTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    inspectionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionConsultationModel", x => x.id);
                    table.ForeignKey(
                        name: "FK_InspectionConsultationModel_InspectionCommentModel_rootComm~",
                        column: x => x.rootCommentid,
                        principalTable: "InspectionCommentModel",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InspectionConsultationModel_Inspection_InspectionModelid",
                        column: x => x.InspectionModelid,
                        principalTable: "Inspection",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_InspectionConsultationModel_Speciality_specialityid",
                        column: x => x.specialityid,
                        principalTable: "Speciality",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InspectionCommentModel_authorid",
                table: "InspectionCommentModel",
                column: "authorid");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionConsultationModel_InspectionModelid",
                table: "InspectionConsultationModel",
                column: "InspectionModelid");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionConsultationModel_rootCommentid",
                table: "InspectionConsultationModel",
                column: "rootCommentid");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionConsultationModel_specialityid",
                table: "InspectionConsultationModel",
                column: "specialityid");
        }
    }
}
