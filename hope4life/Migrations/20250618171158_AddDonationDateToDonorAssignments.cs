using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hope4life.Migrations
{
    /// <inheritdoc />
    public partial class AddDonationDateToDonorAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "DonorAssignments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "DonationDate",
                table: "DonorAssignments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DonorAssignments_DonorId",
                table: "DonorAssignments",
                column: "DonorId");

            migrationBuilder.AddForeignKey(
                name: "FK_DonorAssignments_Donors_DonorId",
                table: "DonorAssignments",
                column: "DonorId",
                principalTable: "Donors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DonorAssignments_Donors_DonorId",
                table: "DonorAssignments");

            migrationBuilder.DropIndex(
                name: "IX_DonorAssignments_DonorId",
                table: "DonorAssignments");

            migrationBuilder.DropColumn(
                name: "DonationDate",
                table: "DonorAssignments");

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "DonorAssignments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
