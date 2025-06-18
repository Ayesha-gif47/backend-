using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hope4life.Migrations
{
    /// <inheritdoc />
    public partial class _ : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmergencyRequests_Patients_PatientId",
                table: "EmergencyRequests");

            migrationBuilder.CreateTable(
                name: "DonorAssignments",
                columns: table => new
                {
                    DonorAssignmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScheduleDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDonated = table.Column<bool>(type: "bit", nullable: false),
                    IsWillingToDonate = table.Column<bool>(type: "bit", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DonorType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonorAssignments", x => x.DonorAssignmentId);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_EmergencyRequests_Patients_PatientId",
                table: "EmergencyRequests",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmergencyRequests_Patients_PatientId",
                table: "EmergencyRequests");

            migrationBuilder.DropTable(
                name: "DonorAssignments");

            migrationBuilder.AddForeignKey(
                name: "FK_EmergencyRequests_Patients_PatientId",
                table: "EmergencyRequests",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
