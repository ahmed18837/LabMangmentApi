using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LabMangmentApi.Migrations
{
    /// <inheritdoc />
    public partial class AddAllRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "22ab6d2a-1b36-4dce-aa6c-546a4cf92759");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c62edd67-cc0c-40eb-aec5-a5aca03f3d48");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0c20a355-12dd-449d-a8d5-6e33960c45ee", "0c20a355-12dd-449d-a8d5-6e33960c45ee", "Student", "STUDENT" },
                    { "393f1091-b175-4cca-a1df-19971e86e2a3", "393f1091-b175-4cca-a1df-19971e86e2a3", "Professor", "PROFESSOR" },
                    { "7d090697-295a-43bf-bb0b-3a19843fb528", "7d090697-295a-43bf-bb0b-3a19843fb528", "Technical", "TECHNICAL" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0c20a355-12dd-449d-a8d5-6e33960c45ee");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "393f1091-b175-4cca-a1df-19971e86e2a3");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7d090697-295a-43bf-bb0b-3a19843fb528");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "22ab6d2a-1b36-4dce-aa6c-546a4cf92759", "22ab6d2a-1b36-4dce-aa6c-546a4cf92759", "Admin", "ADMIN" },
                    { "c62edd67-cc0c-40eb-aec5-a5aca03f3d48", "c62edd67-cc0c-40eb-aec5-a5aca03f3d48", "User", "USER" }
                });
        }
    }
}
