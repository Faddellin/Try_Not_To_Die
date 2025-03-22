using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Try_not_to_DIE.Migrations
{
    /// <inheritdoc />
    public partial class Notificationsaddedv11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "doctorEmail",
                table: "DoctorNotification");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DoctorNotification",
                table: "DoctorNotification",
                column: "inspectionID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DoctorNotification",
                table: "DoctorNotification");

            migrationBuilder.AddColumn<string>(
                name: "doctorEmail",
                table: "DoctorNotification",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
