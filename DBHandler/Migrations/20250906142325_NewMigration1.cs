using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DBHandler.Migrations
{
    /// <inheritdoc />
    public partial class NewMigration1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Responsable",
                table: "EXPEDIENTE_DETALLES");

            migrationBuilder.AddColumn<int>(
                name: "AsesorId",
                table: "EXPEDIENTE_DETALLES",
                type: "NUMBER(10)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EtapaAnteriorId",
                table: "EXPEDIENTE_DETALLES",
                type: "NUMBER(10)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EtapaDetalleAnteriorId",
                table: "EXPEDIENTE_DETALLES",
                type: "NUMBER(10)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EtapaDetalleNuevaId",
                table: "EXPEDIENTE_DETALLES",
                type: "NUMBER(10)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EtapaNuevaId",
                table: "EXPEDIENTE_DETALLES",
                type: "NUMBER(10)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "NombreArchivo",
                table: "EXPEDIENTE_DETALLES",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NombreArchivoHash",
                table: "EXPEDIENTE_DETALLES",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ubicacion",
                table: "EXPEDIENTE_DETALLES",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EXPEDIENTE_DETALLES_AsesorId",
                table: "EXPEDIENTE_DETALLES",
                column: "AsesorId");

            migrationBuilder.AddForeignKey(
                name: "FK_EXPEDIENTE_DETALLES_USUARIOS_AsesorId",
                table: "EXPEDIENTE_DETALLES",
                column: "AsesorId",
                principalTable: "USUARIOS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EXPEDIENTE_DETALLES_USUARIOS_AsesorId",
                table: "EXPEDIENTE_DETALLES");

            migrationBuilder.DropIndex(
                name: "IX_EXPEDIENTE_DETALLES_AsesorId",
                table: "EXPEDIENTE_DETALLES");

            migrationBuilder.DropColumn(
                name: "AsesorId",
                table: "EXPEDIENTE_DETALLES");

            migrationBuilder.DropColumn(
                name: "EtapaAnteriorId",
                table: "EXPEDIENTE_DETALLES");

            migrationBuilder.DropColumn(
                name: "EtapaDetalleAnteriorId",
                table: "EXPEDIENTE_DETALLES");

            migrationBuilder.DropColumn(
                name: "EtapaDetalleNuevaId",
                table: "EXPEDIENTE_DETALLES");

            migrationBuilder.DropColumn(
                name: "EtapaNuevaId",
                table: "EXPEDIENTE_DETALLES");

            migrationBuilder.DropColumn(
                name: "NombreArchivo",
                table: "EXPEDIENTE_DETALLES");

            migrationBuilder.DropColumn(
                name: "NombreArchivoHash",
                table: "EXPEDIENTE_DETALLES");

            migrationBuilder.DropColumn(
                name: "Ubicacion",
                table: "EXPEDIENTE_DETALLES");

            migrationBuilder.AddColumn<string>(
                name: "Responsable",
                table: "EXPEDIENTE_DETALLES",
                type: "NVARCHAR2(2000)",
                nullable: false,
                defaultValue: "");
        }
    }
}
