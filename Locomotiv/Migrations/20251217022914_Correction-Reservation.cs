using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Locomotiv.Migrations
{
    /// <inheritdoc />
    public partial class CorrectionReservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "MontantTotal",
                table: "Reservations",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_EstActif_Statut",
                table: "Reservations",
                columns: new[] { "EstActif", "Statut" });

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_NumeroBillet",
                table: "Reservations",
                column: "NumeroBillet",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reservations_EstActif_Statut",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_NumeroBillet",
                table: "Reservations");

            migrationBuilder.AlterColumn<decimal>(
                name: "MontantTotal",
                table: "Reservations",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}
