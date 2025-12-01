using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Locomotiv.Migrations
{
    /// <inheritdoc />
    public partial class ModifTrainEnums : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trains_Blocks_BlockActuelId",
                table: "Trains");

            migrationBuilder.DropForeignKey(
                name: "FK_Trains_Stations_StationActuelleId",
                table: "Trains");

            migrationBuilder.DropForeignKey(
                name: "FK_Trains_Voies_VoieActuelleId",
                table: "Trains");

            migrationBuilder.AlterColumn<int>(
                name: "VoieActuelleId",
                table: "Trains",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "StationActuelleId",
                table: "Trains",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "BlockActuelId",
                table: "Trains",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Trains_Blocks_BlockActuelId",
                table: "Trains",
                column: "BlockActuelId",
                principalTable: "Blocks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Trains_Stations_StationActuelleId",
                table: "Trains",
                column: "StationActuelleId",
                principalTable: "Stations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Trains_Voies_VoieActuelleId",
                table: "Trains",
                column: "VoieActuelleId",
                principalTable: "Voies",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trains_Blocks_BlockActuelId",
                table: "Trains");

            migrationBuilder.DropForeignKey(
                name: "FK_Trains_Stations_StationActuelleId",
                table: "Trains");

            migrationBuilder.DropForeignKey(
                name: "FK_Trains_Voies_VoieActuelleId",
                table: "Trains");

            migrationBuilder.AlterColumn<int>(
                name: "VoieActuelleId",
                table: "Trains",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "StationActuelleId",
                table: "Trains",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "BlockActuelId",
                table: "Trains",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Trains_Blocks_BlockActuelId",
                table: "Trains",
                column: "BlockActuelId",
                principalTable: "Blocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Trains_Stations_StationActuelleId",
                table: "Trains",
                column: "StationActuelleId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Trains_Voies_VoieActuelleId",
                table: "Trains",
                column: "VoieActuelleId",
                principalTable: "Voies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
