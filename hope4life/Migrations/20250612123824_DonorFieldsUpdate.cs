using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hope4life.Migrations
{
    /// <inheritdoc />
    public partial class DonorFieldsUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "City",
                table: "Donors",
                newName: "Password");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastDonationDate",
                table: "Donors",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Donors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Donors",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsBackupDonor",
                table: "Donors",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBlocked",
                table: "Donors",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextDonationDate",
                table: "Donors",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NotAvailableCount",
                table: "Donors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "Donors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "Donors",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Donors");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Donors");

            migrationBuilder.DropColumn(
                name: "IsBackupDonor",
                table: "Donors");

            migrationBuilder.DropColumn(
                name: "IsBlocked",
                table: "Donors");

            migrationBuilder.DropColumn(
                name: "NextDonationDate",
                table: "Donors");

            migrationBuilder.DropColumn(
                name: "NotAvailableCount",
                table: "Donors");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "Donors");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "Donors");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Donors",
                newName: "City");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastDonationDate",
                table: "Donors",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
