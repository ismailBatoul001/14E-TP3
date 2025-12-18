using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Locomotiv.Migrations
{
    /// <inheritdoc />
    public partial class Initiale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Blocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", nullable: false),
                    LatitudeDebut = table.Column<double>(type: "REAL", nullable: false),
                    LongitudeDebut = table.Column<double>(type: "REAL", nullable: false),
                    LatitudeFin = table.Column<double>(type: "REAL", nullable: false),
                    LongitudeFin = table.Column<double>(type: "REAL", nullable: false),
                    EstOccupe = table.Column<bool>(type: "INTEGER", nullable: false),
                    TrainActuelId = table.Column<int>(type: "INTEGER", nullable: true),
                    BlockId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Blocks_Blocks_BlockId",
                        column: x => x.BlockId,
                        principalTable: "Blocks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PointsInteret",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Latitude = table.Column<double>(type: "REAL", nullable: false),
                    Longitude = table.Column<double>(type: "REAL", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointsInteret", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", nullable: false),
                    Latitude = table.Column<double>(type: "REAL", nullable: false),
                    Longitude = table.Column<double>(type: "REAL", nullable: false),
                    CapaciteMaximale = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Signaux",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", nullable: false),
                    Etat = table.Column<int>(type: "INTEGER", nullable: false),
                    StationId = table.Column<int>(type: "INTEGER", nullable: false),
                    BlockId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Signaux", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Signaux_Blocks_BlockId",
                        column: x => x.BlockId,
                        principalTable: "Blocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Signaux_Stations_StationId",
                        column: x => x.StationId,
                        principalTable: "Stations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Prenom = table.Column<string>(type: "TEXT", nullable: false),
                    Nom = table.Column<string>(type: "TEXT", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<int>(type: "INTEGER", nullable: false),
                    StationAssigneeId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Stations_StationAssigneeId",
                        column: x => x.StationAssigneeId,
                        principalTable: "Stations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Voies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Numero = table.Column<string>(type: "TEXT", nullable: false),
                    EstDisponible = table.Column<bool>(type: "INTEGER", nullable: false),
                    StationId = table.Column<int>(type: "INTEGER", nullable: false),
                    TrainActuelId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Voies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Voies_Stations_StationId",
                        column: x => x.StationId,
                        principalTable: "Stations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Trains",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Numero = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Etat = table.Column<int>(type: "INTEGER", nullable: false),
                    Capacite = table.Column<int>(type: "INTEGER", nullable: false),
                    StationActuelleId = table.Column<int>(type: "INTEGER", nullable: true),
                    VoieActuelleId = table.Column<int>(type: "INTEGER", nullable: true),
                    BlockActuelId = table.Column<int>(type: "INTEGER", nullable: true),
                    NombreWagonsTotal = table.Column<int>(type: "INTEGER", nullable: true),
                    NombreWagonsDisponibles = table.Column<int>(type: "INTEGER", nullable: true),
                    CapaciteChargeTonnes = table.Column<double>(type: "REAL", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trains", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trains_Blocks_BlockActuelId",
                        column: x => x.BlockActuelId,
                        principalTable: "Blocks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Trains_Stations_StationActuelleId",
                        column: x => x.StationActuelleId,
                        principalTable: "Stations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Trains_Voies_VoieActuelleId",
                        column: x => x.VoieActuelleId,
                        principalTable: "Voies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Inspections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TrainId = table.Column<int>(type: "INTEGER", nullable: false),
                    DateInspection = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TypeInspection = table.Column<int>(type: "INTEGER", nullable: false),
                    Resultat = table.Column<int>(type: "INTEGER", nullable: false),
                    Observations = table.Column<string>(type: "TEXT", nullable: false),
                    MecanicienId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inspections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inspections_Trains_TrainId",
                        column: x => x.TrainId,
                        principalTable: "Trains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Inspections_Users_MecanicienId",
                        column: x => x.MecanicienId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Itineraires",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EstActif = table.Column<bool>(type: "INTEGER", nullable: false),
                    TrainId = table.Column<int>(type: "INTEGER", nullable: false),
                    StationDepartId = table.Column<int>(type: "INTEGER", nullable: false),
                    StationArriveeId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Itineraires", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Itineraires_Stations_StationArriveeId",
                        column: x => x.StationArriveeId,
                        principalTable: "Stations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Itineraires_Stations_StationDepartId",
                        column: x => x.StationDepartId,
                        principalTable: "Stations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Itineraires_Trains_TrainId",
                        column: x => x.TrainId,
                        principalTable: "Trains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItineraireArrets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Ordre = table.Column<int>(type: "INTEGER", nullable: false),
                    HeureArrivee = table.Column<DateTime>(type: "TEXT", nullable: false),
                    HeureDepart = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EstStation = table.Column<bool>(type: "INTEGER", nullable: false),
                    ItineraireId = table.Column<int>(type: "INTEGER", nullable: false),
                    StationId = table.Column<int>(type: "INTEGER", nullable: true),
                    PointInteretId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItineraireArrets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItineraireArrets_Itineraires_ItineraireId",
                        column: x => x.ItineraireId,
                        principalTable: "Itineraires",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItineraireArrets_PointsInteret_PointInteretId",
                        column: x => x.PointInteretId,
                        principalTable: "PointsInteret",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItineraireArrets_Stations_StationId",
                        column: x => x.StationId,
                        principalTable: "Stations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DateReservation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EstActif = table.Column<bool>(type: "INTEGER", nullable: false),
                    Statut = table.Column<int>(type: "INTEGER", nullable: false),
                    NombrePassagers = table.Column<int>(type: "INTEGER", nullable: false),
                    MontantTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ItineraireId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    NumeroBillet = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    DateAnnulation = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservations_Itineraires_ItineraireId",
                        column: x => x.ItineraireId,
                        principalTable: "Itineraires",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reservations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                name: "IX_Blocks_BlockId",
                table: "Blocks",
                column: "BlockId");

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_MecanicienId",
                table: "Inspections",
                column: "MecanicienId");

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_TrainId",
                table: "Inspections",
                column: "TrainId");

            migrationBuilder.CreateIndex(
                name: "IX_ItineraireArrets_ItineraireId",
                table: "ItineraireArrets",
                column: "ItineraireId");

            migrationBuilder.CreateIndex(
                name: "IX_ItineraireArrets_PointInteretId",
                table: "ItineraireArrets",
                column: "PointInteretId");

            migrationBuilder.CreateIndex(
                name: "IX_ItineraireArrets_StationId",
                table: "ItineraireArrets",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_Itineraires_StationArriveeId",
                table: "Itineraires",
                column: "StationArriveeId");

            migrationBuilder.CreateIndex(
                name: "IX_Itineraires_StationDepartId",
                table: "Itineraires",
                column: "StationDepartId");

            migrationBuilder.CreateIndex(
                name: "IX_Itineraires_TrainId",
                table: "Itineraires",
                column: "TrainId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_EstActif_Statut",
                table: "Reservations",
                columns: new[] { "EstActif", "Statut" });

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ItineraireId",
                table: "Reservations",
                column: "ItineraireId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_NumeroBillet",
                table: "Reservations",
                column: "NumeroBillet",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_UserId",
                table: "Reservations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationsWagons_ClientCommercialId",
                table: "ReservationsWagons",
                column: "ClientCommercialId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationsWagons_ItineraireId",
                table: "ReservationsWagons",
                column: "ItineraireId");

            migrationBuilder.CreateIndex(
                name: "IX_Signaux_BlockId",
                table: "Signaux",
                column: "BlockId");

            migrationBuilder.CreateIndex(
                name: "IX_Signaux_StationId",
                table: "Signaux",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_Trains_BlockActuelId",
                table: "Trains",
                column: "BlockActuelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Trains_StationActuelleId",
                table: "Trains",
                column: "StationActuelleId");

            migrationBuilder.CreateIndex(
                name: "IX_Trains_VoieActuelleId",
                table: "Trains",
                column: "VoieActuelleId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_StationAssigneeId",
                table: "Users",
                column: "StationAssigneeId");

            migrationBuilder.CreateIndex(
                name: "IX_Voies_StationId",
                table: "Voies",
                column: "StationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Inspections");

            migrationBuilder.DropTable(
                name: "ItineraireArrets");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "ReservationsWagons");

            migrationBuilder.DropTable(
                name: "Signaux");

            migrationBuilder.DropTable(
                name: "PointsInteret");

            migrationBuilder.DropTable(
                name: "Itineraires");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Trains");

            migrationBuilder.DropTable(
                name: "Blocks");

            migrationBuilder.DropTable(
                name: "Voies");

            migrationBuilder.DropTable(
                name: "Stations");
        }
    }
}
