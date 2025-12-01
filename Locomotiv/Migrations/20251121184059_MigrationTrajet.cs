using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Locomotiv.Migrations
{
    /// <inheritdoc />
    public partial class MigrationTrajet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItineraireArrets_PointsInteret_PointInteretId",
                table: "ItineraireArrets");

            migrationBuilder.DropForeignKey(
                name: "FK_ItineraireArrets_Stations_StationId",
                table: "ItineraireArrets");

            migrationBuilder.AddForeignKey(
                name: "FK_ItineraireArrets_PointsInteret_PointInteretId",
                table: "ItineraireArrets",
                column: "PointInteretId",
                principalTable: "PointsInteret",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ItineraireArrets_Stations_StationId",
                table: "ItineraireArrets",
                column: "StationId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItineraireArrets_PointsInteret_PointInteretId",
                table: "ItineraireArrets");

            migrationBuilder.DropForeignKey(
                name: "FK_ItineraireArrets_Stations_StationId",
                table: "ItineraireArrets");

            migrationBuilder.AddForeignKey(
                name: "FK_ItineraireArrets_PointsInteret_PointInteretId",
                table: "ItineraireArrets",
                column: "PointInteretId",
                principalTable: "PointsInteret",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItineraireArrets_Stations_StationId",
                table: "ItineraireArrets",
                column: "StationId",
                principalTable: "Stations",
                principalColumn: "Id");
        }
    }
}
