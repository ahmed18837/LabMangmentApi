using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMangmentApi.Migrations
{
    /// <inheritdoc />
    public partial class remove : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Prpose",
                table: "Reservations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Prpose",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
