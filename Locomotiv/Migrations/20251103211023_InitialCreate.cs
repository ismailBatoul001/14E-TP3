using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Locomotiv.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
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
                    EstOccupe = table.Column<bool>(type: "INTEGER", nullable: false),
                    LatitudeDebut = table.Column<double>(type: "REAL", nullable: false),
                    LongitudeDebut = table.Column<double>(type: "REAL", nullable: false),
                    LatitudeFin = table.Column<double>(type: "REAL", nullable: false),
                    LongitudeFin = table.Column<double>(type: "REAL", nullable: false),
                    TrainActuelId = table.Column<int>(type: "INTEGER", nullable: false),
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
                    StationActuelleId = table.Column<int>(type: "INTEGER", nullable: false),
                    VoieActuelleId = table.Column<int>(type: "INTEGER", nullable: false),
                    BlockActuelId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trains", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trains_Blocks_BlockActuelId",
                        column: x => x.BlockActuelId,
                        principalTable: "Blocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Trains_Stations_StationActuelleId",
                        column: x => x.StationActuelleId,
                        principalTable: "Stations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Trains_Voies_VoieActuelleId",
                        column: x => x.VoieActuelleId,
                        principalTable: "Voies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    StationId = table.Column<int>(type: "INTEGER", nullable: false),
                    PointInteretId = table.Column<int>(type: "INTEGER", nullable: false)
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
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItineraireArrets_Stations_StationId",
                        column: x => x.StationId,
                        principalTable: "Stations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Blocks_BlockId",
                table: "Blocks",
                column: "BlockId");

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
                name: "ItineraireArrets");

            migrationBuilder.DropTable(
                name: "Signaux");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Itineraires");

            migrationBuilder.DropTable(
                name: "PointsInteret");

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
