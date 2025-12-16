using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Locomotiv.Migrations
{
    /// <inheritdoc />
    public partial class ReservationWagon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "CapaciteChargeTonnes",
                table: "Trains",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NombreWagonsDisponibles",
                table: "Trains",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NombreWagonsTotal",
                table: "Trains",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ReservationsWagons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClientCommercialId = table.Column<int>(type: "INTEGER", nullable: false),
                    ItineraireId = table.Column<int>(type: "INTEGER", nullable: false),
                    NombreWagons = table.Column<int>(type: "INTEGER", nullable: false),
                    TypeWagon = table.Column<int>(type: "INTEGER", nullable: false),
                    PoidsTotal = table.Column<double>(type: "decimal(18,2)", nullable: false),
                    TarifTotal = table.Column<double>(type: "decimal(18,2)", nullable: false),
                    DateReservation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Statut = table.Column<int>(type: "INTEGER", nullable: false),
                    NotesSpeciales = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationsWagons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReservationsWagons_Itineraires_ItineraireId",
                        column: x => x.ItineraireId,
                        principalTable: "Itineraires",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReservationsWagons_Users_ClientCommercialId",
                        column: x => x.ClientCommercialId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReservationsWagons_ClientCommercialId",
                table: "ReservationsWagons",
                column: "ClientCommercialId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationsWagons_ItineraireId",
                table: "ReservationsWagons",
                column: "ItineraireId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReservationsWagons");

            migrationBuilder.DropColumn(
                name: "CapaciteChargeTonnes",
                table: "Trains");

            migrationBuilder.DropColumn(
                name: "NombreWagonsDisponibles",
                table: "Trains");

            migrationBuilder.DropColumn(
                name: "NombreWagonsTotal",
                table: "Trains");
        }
    }
}
