using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagement.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAppointmentEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppointmentProcedures");

            migrationBuilder.DropTable(
                name: "Treatment");

            migrationBuilder.DropTable(
                name: "Appointment");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Appointment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DoctorId = table.Column<int>(type: "int", nullable: false),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    Duration = table.Column<TimeSpan>(type: "time", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Appointm__3214EC07E4B95527", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Appointme__Docto__3B75D760",
                        column: x => x.DoctorId,
                        principalTable: "Doctor",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Appointme__Patie__3A81B327",
                        column: x => x.PatientId,
                        principalTable: "Patient",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AppointmentProcedures",
                columns: table => new
                {
                    AppointmentId = table.Column<int>(type: "int", nullable: false),
                    ProcedureId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentProcedures", x => new { x.AppointmentId, x.ProcedureId });
                    table.ForeignKey(
                        name: "FK_AppointmentProcedures_Appointment_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppointmentProcedures_Procedures_ProcedureId",
                        column: x => x.ProcedureId,
                        principalTable: "Procedures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Treatment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Medication = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Treatment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Treatment_Appointment_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_DoctorId",
                table: "Appointment",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_PatientId",
                table: "Appointment",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentProcedures_ProcedureId",
                table: "AppointmentProcedures",
                column: "ProcedureId");

            migrationBuilder.CreateIndex(
                name: "IX_Treatment_AppointmentId",
                table: "Treatment",
                column: "AppointmentId",
                unique: true);
        }
    }
}
