using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DBHandler.Migrations.DBHandler
{
    /// <inheritdoc />
    public partial class QueryCompleto3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EXPEDIENTES_EXPEDIENTES_ExpedienteRelacionadoId",
                table: "EXPEDIENTES");

            migrationBuilder.AlterColumn<int>(
                name: "ExpedienteRelacionadoId",
                table: "EXPEDIENTES",
                type: "NUMBER(10)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "NUMBER(10)");

            migrationBuilder.AddForeignKey(
                name: "FK_EXPEDIENTES_EXPEDIENTES_ExpedienteRelacionadoId",
                table: "EXPEDIENTES",
                column: "ExpedienteRelacionadoId",
                principalTable: "EXPEDIENTES",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EXPEDIENTES_EXPEDIENTES_ExpedienteRelacionadoId",
                table: "EXPEDIENTES");

            migrationBuilder.AlterColumn<int>(
                name: "ExpedienteRelacionadoId",
                table: "EXPEDIENTES",
                type: "NUMBER(10)",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "NUMBER(10)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EXPEDIENTES_EXPEDIENTES_ExpedienteRelacionadoId",
                table: "EXPEDIENTES",
                column: "ExpedienteRelacionadoId",
                principalTable: "EXPEDIENTES",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
