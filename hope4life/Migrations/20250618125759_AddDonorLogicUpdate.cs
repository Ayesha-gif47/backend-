using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hope4life.Migrations
{
    /// <inheritdoc />
    public partial class AddDonorLogicUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DonorAssignments",
                table: "DonorAssignments");

            migrationBuilder.DropColumn(
                name: "DonorAssignmentId",
                table: "DonorAssignments");

            migrationBuilder.DropColumn(
                name: "PatientRequestId",
                table: "DonorAssignments");

            migrationBuilder.RenameColumn(
                name: "FrequencyInWeeks",
                table: "Patients",
                newName: "Frequency");

            migrationBuilder.RenameColumn(
                name: "IsDonated",
                table: "DonorAssignments",
                newName: "Donated");

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "DonorAssignments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "DonorAssignments",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DonorAssignments",
                table: "DonorAssignments",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DonorAssignments_PatientId",
                table: "DonorAssignments",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_DonorAssignments_Patients_PatientId",
                table: "DonorAssignments",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DonorAssignments_Patients_PatientId",
                table: "DonorAssignments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DonorAssignments",
                table: "DonorAssignments");

            migrationBuilder.DropIndex(
                name: "IX_DonorAssignments_PatientId",
                table: "DonorAssignments");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "DonorAssignments");

            migrationBuilder.RenameColumn(
                name: "Frequency",
                table: "Patients",
                newName: "FrequencyInWeeks");

            migrationBuilder.RenameColumn(
                name: "Donated",
                table: "DonorAssignments",
                newName: "IsDonated");

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "DonorAssignments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "DonorAssignmentId",
                table: "DonorAssignments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PatientRequestId",
                table: "DonorAssignments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DonorAssignments",
                table: "DonorAssignments",
                column: "DonorAssignmentId");
        }
    }
}
