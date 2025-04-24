using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMangmentApi.Migrations
{
    /// <inheritdoc />
    public partial class addPurpose : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Purpose",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Purpose",
                table: "Reservations");
        }
    }
}
