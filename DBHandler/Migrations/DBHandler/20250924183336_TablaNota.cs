using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DBHandler.Migrations.DBHandler
{
    /// <inheritdoc />
    public partial class TablaNota : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_EXPEDIENTE_DETALLES_AsesorAnteriorId",
                table: "EXPEDIENTE_DETALLES",
                column: "AsesorAnteriorId");

            migrationBuilder.CreateIndex(
                name: "IX_EXPEDIENTE_DETALLES_AsesorNuevorId",
                table: "EXPEDIENTE_DETALLES",
                column: "AsesorNuevorId");

            migrationBuilder.AddForeignKey(
                name: "FK_EXPEDIENTE_DETALLES_USUARIOS_AsesorAnteriorId",
                table: "EXPEDIENTE_DETALLES",
                column: "AsesorAnteriorId",
                principalTable: "USUARIOS",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EXPEDIENTE_DETALLES_USUARIOS_AsesorNuevorId",
                table: "EXPEDIENTE_DETALLES",
                column: "AsesorNuevorId",
                principalTable: "USUARIOS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EXPEDIENTE_DETALLES_USUARIOS_AsesorAnteriorId",
                table: "EXPEDIENTE_DETALLES");

            migrationBuilder.DropForeignKey(
                name: "FK_EXPEDIENTE_DETALLES_USUARIOS_AsesorNuevorId",
                table: "EXPEDIENTE_DETALLES");

            migrationBuilder.DropIndex(
                name: "IX_EXPEDIENTE_DETALLES_AsesorAnteriorId",
                table: "EXPEDIENTE_DETALLES");

            migrationBuilder.DropIndex(
                name: "IX_EXPEDIENTE_DETALLES_AsesorNuevorId",
                table: "EXPEDIENTE_DETALLES");
        }
    }
}
