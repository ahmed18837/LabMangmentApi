using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMangmentApi.Migrations
{
    /// <inheritdoc />
    public partial class updatedevicedata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Coast",
                table: "Maintenances",
                newName: "Cost");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastMaintenanceDate",
                table: "Devices",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Cost",
                table: "Maintenances",
                newName: "Coast");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastMaintenanceDate",
                table: "Devices",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
