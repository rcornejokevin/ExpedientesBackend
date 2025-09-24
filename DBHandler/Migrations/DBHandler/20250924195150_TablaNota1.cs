using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DBHandler.Migrations.DBHandler
{
    /// <inheritdoc />
    public partial class TablaNota1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EXPEDIENTE_NOTAS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ExpedienteId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    FechaIngreso = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    AsesorId = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    Nota = table.Column<string>(type: "CLOB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EXPEDIENTE_NOTAS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EXPEDIENTE_NOTAS_USUARIOS_AsesorId",
                        column: x => x.AsesorId,
                        principalTable: "USUARIOS",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EXPEDIENTE_NOTAS_AsesorId",
                table: "EXPEDIENTE_NOTAS",
                column: "AsesorId");

            migrationBuilder.CreateIndex(
                name: "IX_EXPEDIENTE_NOTAS_ExpedienteId",
                table: "EXPEDIENTE_NOTAS",
                column: "ExpedienteId");

            migrationBuilder.CreateIndex(
                name: "IX_EXPEDIENTE_NOTAS_FechaIngreso",
                table: "EXPEDIENTE_NOTAS",
                column: "FechaIngreso");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EXPEDIENTE_NOTAS");
        }
    }
}
