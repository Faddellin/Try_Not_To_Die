using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Try_not_to_DIE.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Icd10",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    createTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    code = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Icd10", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Patient",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    createTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    birthday = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    gender = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patient", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Speciality",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    createTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Speciality", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Consultation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    createTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    inspectionId = table.Column<Guid>(type: "uuid", nullable: false),
                    specialityid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consultation", x => x.id);
                    table.ForeignKey(
                        name: "FK_Consultation_Speciality_specialityid",
                        column: x => x.specialityid,
                        principalTable: "Speciality",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Doctor",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    createTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    birthday = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    gender = table.Column<int>(type: "integer", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    phone = table.Column<string>(type: "text", nullable: true),
                    specialityid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doctor", x => x.id);
                    table.ForeignKey(
                        name: "FK_Doctor_Speciality_specialityid",
                        column: x => x.specialityid,
                        principalTable: "Speciality",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    createTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    content = table.Column<string>(type: "text", nullable: false),
                    authorId = table.Column<Guid>(type: "uuid", nullable: false),
                    author = table.Column<string>(type: "text", nullable: false),
                    parentId = table.Column<Guid>(type: "uuid", nullable: true),
                    ConsultationModelid = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.id);
                    table.ForeignKey(
                        name: "FK_Comment_Consultation_ConsultationModelid",
                        column: x => x.ConsultationModelid,
                        principalTable: "Consultation",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Inspection",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    createTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    anamnesis = table.Column<string>(type: "text", nullable: true),
                    complaints = table.Column<string>(type: "text", nullable: true),
                    treatment = table.Column<string>(type: "text", nullable: true),
                    conclusion = table.Column<int>(type: "integer", nullable: false),
                    nextVisitDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deathDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    baseInspectionId = table.Column<Guid>(type: "uuid", nullable: true),
                    previousInspectionId = table.Column<Guid>(type: "uuid", nullable: true),
                    patientid = table.Column<Guid>(type: "uuid", nullable: false),
                    doctorid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inspection", x => x.id);
                    table.ForeignKey(
                        name: "FK_Inspection_Doctor_doctorid",
                        column: x => x.doctorid,
                        principalTable: "Doctor",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Inspection_Patient_patientid",
                        column: x => x.patientid,
                        principalTable: "Patient",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InspectionCommentModel",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    createTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    parentId = table.Column<Guid>(type: "uuid", nullable: true),
                    content = table.Column<string>(type: "text", nullable: true),
                    authorid = table.Column<Guid>(type: "uuid", nullable: false),
                    modifyTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                name: "Diagnosis",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    createTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    code = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<int>(type: "integer", nullable: false),
                    InspectionModelid = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diagnosis", x => x.id);
                    table.ForeignKey(
                        name: "FK_Diagnosis_Inspection_InspectionModelid",
                        column: x => x.InspectionModelid,
                        principalTable: "Inspection",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "InspectionConsultationModel",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    createTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    inspectionId = table.Column<Guid>(type: "uuid", nullable: false),
                    specialityid = table.Column<Guid>(type: "uuid", nullable: false),
                    rootCommentid = table.Column<Guid>(type: "uuid", nullable: false),
                    commentsNumber = table.Column<int>(type: "integer", nullable: false),
                    InspectionModelid = table.Column<Guid>(type: "uuid", nullable: true)
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
                name: "IX_Comment_ConsultationModelid",
                table: "Comment",
                column: "ConsultationModelid");

            migrationBuilder.CreateIndex(
                name: "IX_Consultation_specialityid",
                table: "Consultation",
                column: "specialityid");

            migrationBuilder.CreateIndex(
                name: "IX_Diagnosis_InspectionModelid",
                table: "Diagnosis",
                column: "InspectionModelid");

            migrationBuilder.CreateIndex(
                name: "IX_Doctor_specialityid",
                table: "Doctor",
                column: "specialityid");

            migrationBuilder.CreateIndex(
                name: "IX_Inspection_doctorid",
                table: "Inspection",
                column: "doctorid");

            migrationBuilder.CreateIndex(
                name: "IX_Inspection_patientid",
                table: "Inspection",
                column: "patientid");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.DropTable(
                name: "Diagnosis");

            migrationBuilder.DropTable(
                name: "Icd10");

            migrationBuilder.DropTable(
                name: "InspectionConsultationModel");

            migrationBuilder.DropTable(
                name: "Consultation");

            migrationBuilder.DropTable(
                name: "InspectionCommentModel");

            migrationBuilder.DropTable(
                name: "Inspection");

            migrationBuilder.DropTable(
                name: "Doctor");

            migrationBuilder.DropTable(
                name: "Patient");

            migrationBuilder.DropTable(
                name: "Speciality");
        }
    }
}
