using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Try_not_to_DIE.Migrations
{
    /// <inheritdoc />
    public partial class User_auth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isAuthorized",
                table: "Doctor",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isAuthorized",
                table: "Doctor");
        }
    }
}
