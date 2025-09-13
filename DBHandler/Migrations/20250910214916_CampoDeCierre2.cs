using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DBHandler.Migrations
{
    /// <inheritdoc />
    public partial class CampoDeCierre2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Asunto",
                table: "EXPEDIENTES",
                type: "NVARCHAR2(2000)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RemitenteId",
                table: "EXPEDIENTES",
                type: "NUMBER(10)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_EXPEDIENTES_RemitenteId",
                table: "EXPEDIENTES",
                column: "RemitenteId");

            migrationBuilder.AddForeignKey(
                name: "FK_EXPEDIENTES_REMITENTE_RemitenteId",
                table: "EXPEDIENTES",
                column: "RemitenteId",
                principalTable: "REMITENTE",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EXPEDIENTES_REMITENTE_RemitenteId",
                table: "EXPEDIENTES");

            migrationBuilder.DropIndex(
                name: "IX_EXPEDIENTES_RemitenteId",
                table: "EXPEDIENTES");

            migrationBuilder.DropColumn(
                name: "Asunto",
                table: "EXPEDIENTES");

            migrationBuilder.DropColumn(
                name: "RemitenteId",
                table: "EXPEDIENTES");
        }
    }
}
