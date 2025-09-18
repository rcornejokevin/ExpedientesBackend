using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DBHandler.Migrations.DBHandler
{
    /// <inheritdoc />
    public partial class QueryCompleto2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExpedienteRelacionadoId",
                table: "EXPEDIENTES",
                type: "NUMBER(10)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_EXPEDIENTES_ExpedienteRelacionadoId",
                table: "EXPEDIENTES",
                column: "ExpedienteRelacionadoId");

            migrationBuilder.AddForeignKey(
                name: "FK_EXPEDIENTES_EXPEDIENTES_ExpedienteRelacionadoId",
                table: "EXPEDIENTES",
                column: "ExpedienteRelacionadoId",
                principalTable: "EXPEDIENTES",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EXPEDIENTES_EXPEDIENTES_ExpedienteRelacionadoId",
                table: "EXPEDIENTES");

            migrationBuilder.DropIndex(
                name: "IX_EXPEDIENTES_ExpedienteRelacionadoId",
                table: "EXPEDIENTES");

            migrationBuilder.DropColumn(
                name: "ExpedienteRelacionadoId",
                table: "EXPEDIENTES");
        }
    }
}
