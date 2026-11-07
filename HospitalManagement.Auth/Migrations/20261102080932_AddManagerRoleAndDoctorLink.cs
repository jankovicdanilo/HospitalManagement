using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagement.Auth.Migrations
{
    /// <inheritdoc />
    public partial class AddManagerRoleAndDoctorLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DoctorId",
                table: "User",
                type: "int",
                nullable: true);

            migrationBuilder.Sql("UPDATE [User] SET [Role] = 'Manager' WHERE [Role] = 'Receptionist'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE [User] SET [Role] = 'Receptionist' WHERE [Role] = 'Manager'");

            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "User");
        }
    }
}
