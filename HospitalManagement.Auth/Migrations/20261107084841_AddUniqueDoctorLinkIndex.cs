using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagement.Auth.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueDoctorLinkIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_User_DoctorId",
                table: "User",
                column: "DoctorId",
                unique: true,
                filter: "[DoctorId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_User_DoctorId",
                table: "User");
        }
    }
}
