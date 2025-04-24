using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LabMangmentApi.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NationalId",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "22ab6d2a-1b36-4dce-aa6c-546a4cf92759", "22ab6d2a-1b36-4dce-aa6c-546a4cf92759", "Admin", "ADMIN" },
                    { "c62edd67-cc0c-40eb-aec5-a5aca03f3d48", "c62edd67-cc0c-40eb-aec5-a5aca03f3d48", "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "22ab6d2a-1b36-4dce-aa6c-546a4cf92759");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c62edd67-cc0c-40eb-aec5-a5aca03f3d48");

            migrationBuilder.DropColumn(
                name: "NationalId",
                table: "AspNetUsers");
        }
    }
}
