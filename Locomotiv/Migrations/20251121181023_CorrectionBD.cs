using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Locomotiv.Migrations
{
    /// <inheritdoc />
    public partial class CorrectionBD : Migration
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

            migrationBuilder.AlterColumn<int>(
                name: "StationId",
                table: "ItineraireArrets",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "PointInteretId",
                table: "ItineraireArrets",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItineraireArrets_PointsInteret_PointInteretId",
                table: "ItineraireArrets");

            migrationBuilder.DropForeignKey(
                name: "FK_ItineraireArrets_Stations_StationId",
                table: "ItineraireArrets");

            migrationBuilder.AlterColumn<int>(
                name: "StationId",
                table: "ItineraireArrets",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PointInteretId",
                table: "ItineraireArrets",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ItineraireArrets_PointsInteret_PointInteretId",
                table: "ItineraireArrets",
                column: "PointInteretId",
                principalTable: "PointsInteret",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItineraireArrets_Stations_StationId",
                table: "ItineraireArrets",
                column: "StationId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
