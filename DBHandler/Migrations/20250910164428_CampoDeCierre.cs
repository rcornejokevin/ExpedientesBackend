using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DBHandler.Migrations
{
    /// <inheritdoc />
    public partial class CampoDeCierre : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "FinDeFlujo",
                table: "ETAPAS",
                type: "BOOLEAN",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_EXPEDIENTE_DETALLES_EtapaAnteriorId",
                table: "EXPEDIENTE_DETALLES",
                column: "EtapaAnteriorId");

            migrationBuilder.CreateIndex(
                name: "IX_EXPEDIENTE_DETALLES_EtapaDetalleAnteriorId",
                table: "EXPEDIENTE_DETALLES",
                column: "EtapaDetalleAnteriorId");

            migrationBuilder.CreateIndex(
                name: "IX_EXPEDIENTE_DETALLES_EtapaDetalleNuevaId",
                table: "EXPEDIENTE_DETALLES",
                column: "EtapaDetalleNuevaId");

            migrationBuilder.CreateIndex(
                name: "IX_EXPEDIENTE_DETALLES_EtapaNuevaId",
                table: "EXPEDIENTE_DETALLES",
                column: "EtapaNuevaId");

            migrationBuilder.AddForeignKey(
                name: "FK_EXPEDIENTE_DETALLES_ETAPAS_EtapaAnteriorId",
                table: "EXPEDIENTE_DETALLES",
                column: "EtapaAnteriorId",
                principalTable: "ETAPAS",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EXPEDIENTE_DETALLES_ETAPAS_EtapaNuevaId",
                table: "EXPEDIENTE_DETALLES",
                column: "EtapaNuevaId",
                principalTable: "ETAPAS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EXPEDIENTE_DETALLES_ETAPA_DETALLES_EtapaDetalleAnteriorId",
                table: "EXPEDIENTE_DETALLES",
                column: "EtapaDetalleAnteriorId",
                principalTable: "ETAPA_DETALLES",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EXPEDIENTE_DETALLES_ETAPA_DETALLES_EtapaDetalleNuevaId",
                table: "EXPEDIENTE_DETALLES",
                column: "EtapaDetalleNuevaId",
                principalTable: "ETAPA_DETALLES",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EXPEDIENTE_DETALLES_ETAPAS_EtapaAnteriorId",
                table: "EXPEDIENTE_DETALLES");

            migrationBuilder.DropForeignKey(
                name: "FK_EXPEDIENTE_DETALLES_ETAPAS_EtapaNuevaId",
                table: "EXPEDIENTE_DETALLES");

            migrationBuilder.DropForeignKey(
                name: "FK_EXPEDIENTE_DETALLES_ETAPA_DETALLES_EtapaDetalleAnteriorId",
                table: "EXPEDIENTE_DETALLES");

            migrationBuilder.DropForeignKey(
                name: "FK_EXPEDIENTE_DETALLES_ETAPA_DETALLES_EtapaDetalleNuevaId",
                table: "EXPEDIENTE_DETALLES");

            migrationBuilder.DropIndex(
                name: "IX_EXPEDIENTE_DETALLES_EtapaAnteriorId",
                table: "EXPEDIENTE_DETALLES");

            migrationBuilder.DropIndex(
                name: "IX_EXPEDIENTE_DETALLES_EtapaDetalleAnteriorId",
                table: "EXPEDIENTE_DETALLES");

            migrationBuilder.DropIndex(
                name: "IX_EXPEDIENTE_DETALLES_EtapaDetalleNuevaId",
                table: "EXPEDIENTE_DETALLES");

            migrationBuilder.DropIndex(
                name: "IX_EXPEDIENTE_DETALLES_EtapaNuevaId",
                table: "EXPEDIENTE_DETALLES");

            migrationBuilder.DropColumn(
                name: "FinDeFlujo",
                table: "ETAPAS");
        }
    }
}
