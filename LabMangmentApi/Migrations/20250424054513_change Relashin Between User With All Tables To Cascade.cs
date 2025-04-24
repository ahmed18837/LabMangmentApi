using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMangmentApi.Migrations
{
    /// <inheritdoc />
    public partial class changeRelashinBetweenUserWithAllTablesToCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AlertRecipients_Users_UserId",
                table: "AlertRecipients");

            migrationBuilder.DropForeignKey(
                name: "FK_Maintenances_Users_UserId",
                table: "Maintenances");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Users_UserId",
                table: "Reservations");

            migrationBuilder.AddForeignKey(
                name: "FK_AlertRecipients_Users_UserId",
                table: "AlertRecipients",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Maintenances_Users_UserId",
                table: "Maintenances",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Users_UserId",
                table: "Reservations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AlertRecipients_Users_UserId",
                table: "AlertRecipients");

            migrationBuilder.DropForeignKey(
                name: "FK_Maintenances_Users_UserId",
                table: "Maintenances");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Users_UserId",
                table: "Reservations");

            migrationBuilder.AddForeignKey(
                name: "FK_AlertRecipients_Users_UserId",
                table: "AlertRecipients",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Maintenances_Users_UserId",
                table: "Maintenances",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Users_UserId",
                table: "Reservations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
