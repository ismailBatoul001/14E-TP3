using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Locomotiv.Migrations
{
    /// <inheritdoc />
    public partial class AddInspection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_MecanicienId",
                table: "Inspections",
                column: "MecanicienId");

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_TrainId",
                table: "Inspections",
                column: "TrainId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Inspections");
        }
    }
}
