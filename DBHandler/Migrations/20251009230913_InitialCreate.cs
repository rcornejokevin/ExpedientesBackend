using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DBHandler.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FLUJOS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nombre = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Correlativo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Detalle = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Activo = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CierreArchivado = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CierreDevolucionAlRemitente = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CierreEnviadoAJudicial = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    FlujoAsociado = table.Column<bool>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FLUJOS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "REMITENTE",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Descripcion = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Activo = table.Column<bool>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REMITENTE", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "USUARIOS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Username = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Perfil = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Operativo = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    Activo = table.Column<bool>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USUARIOS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ETAPAS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nombre = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Orden = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Detalle = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Activo = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    FinDeFlujo = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    FlujoId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ETAPAS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ETAPAS_FLUJOS_FlujoId",
                        column: x => x.FlujoId,
                        principalTable: "FLUJOS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "CAMPOS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nombre = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Label = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Placeholder = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    Opciones = table.Column<string>(type: "CLOB", nullable: true),
                    Orden = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Tipo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Activo = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    Requerido = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    Editable = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    FlujoId = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    EtapaId = table.Column<int>(type: "NUMBER(10)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CAMPOS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CAMPOS_ETAPAS_EtapaId",
                        column: x => x.EtapaId,
                        principalTable: "ETAPAS",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CAMPOS_FLUJOS_FlujoId",
                        column: x => x.FlujoId,
                        principalTable: "FLUJOS",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ETAPA_DETALLES",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nombre = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Orden = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Activo = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    Detalle = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    EtapaId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ETAPA_DETALLES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ETAPA_DETALLES_ETAPAS_EtapaId",
                        column: x => x.EtapaId,
                        principalTable: "ETAPAS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EXPEDIENTES",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Codigo = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Asunto = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Estatus = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    FechaIngreso = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    Activo = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    Ubicacion = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CampoValorJson = table.Column<string>(type: "CLOB", nullable: true),
                    NombreArchivo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    NombreArchivoHash = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    EtapaId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    EtapaDetalleId = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    AsesorId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ExpedienteRelacionadoId = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    RemitenteId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EXPEDIENTES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EXPEDIENTES_ETAPAS_EtapaId",
                        column: x => x.EtapaId,
                        principalTable: "ETAPAS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EXPEDIENTES_ETAPA_DETALLES_EtapaDetalleId",
                        column: x => x.EtapaDetalleId,
                        principalTable: "ETAPA_DETALLES",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EXPEDIENTES_EXPEDIENTES_ExpedienteRelacionadoId",
                        column: x => x.ExpedienteRelacionadoId,
                        principalTable: "EXPEDIENTES",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EXPEDIENTES_REMITENTE_RemitenteId",
                        column: x => x.RemitenteId,
                        principalTable: "REMITENTE",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EXPEDIENTES_USUARIOS_AsesorId",
                        column: x => x.AsesorId,
                        principalTable: "USUARIOS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EXPEDIENTE_DETALLES",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Fecha = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Ubicacion = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    NombreArchivo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    EstatusAnterior = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    EstatusNuevo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    NombreArchivoHash = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    EtapaAnteriorId = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    EtapaDetalleAnteriorId = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    EtapaNuevaId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    EtapaDetalleNuevaId = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    AsesorAnteriorId = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    AsesorNuevorId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ExpedienteId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EXPEDIENTE_DETALLES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EXPEDIENTE_DETALLES_ETAPAS_EtapaAnteriorId",
                        column: x => x.EtapaAnteriorId,
                        principalTable: "ETAPAS",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EXPEDIENTE_DETALLES_ETAPAS_EtapaNuevaId",
                        column: x => x.EtapaNuevaId,
                        principalTable: "ETAPAS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EXPEDIENTE_DETALLES_ETAPA_DETALLES_EtapaDetalleAnteriorId",
                        column: x => x.EtapaDetalleAnteriorId,
                        principalTable: "ETAPA_DETALLES",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EXPEDIENTE_DETALLES_ETAPA_DETALLES_EtapaDetalleNuevaId",
                        column: x => x.EtapaDetalleNuevaId,
                        principalTable: "ETAPA_DETALLES",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EXPEDIENTE_DETALLES_EXPEDIENTES_ExpedienteId",
                        column: x => x.ExpedienteId,
                        principalTable: "EXPEDIENTES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EXPEDIENTE_DETALLES_USUARIOS_AsesorAnteriorId",
                        column: x => x.AsesorAnteriorId,
                        principalTable: "USUARIOS",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EXPEDIENTE_DETALLES_USUARIOS_AsesorNuevorId",
                        column: x => x.AsesorNuevorId,
                        principalTable: "USUARIOS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CAMPOS_Activo",
                table: "CAMPOS",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_CAMPOS_EtapaId",
                table: "CAMPOS",
                column: "EtapaId");

            migrationBuilder.CreateIndex(
                name: "IX_CAMPOS_FlujoId",
                table: "CAMPOS",
                column: "FlujoId");

            migrationBuilder.CreateIndex(
                name: "IX_ETAPA_DETALLES_Activo",
                table: "ETAPA_DETALLES",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_ETAPA_DETALLES_EtapaId",
                table: "ETAPA_DETALLES",
                column: "EtapaId");

            migrationBuilder.CreateIndex(
                name: "IX_ETAPAS_Activo",
                table: "ETAPAS",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_ETAPAS_FlujoId",
                table: "ETAPAS",
                column: "FlujoId");

            migrationBuilder.CreateIndex(
                name: "IX_EXPEDIENTE_DETALLES_AsesorAnteriorId",
                table: "EXPEDIENTE_DETALLES",
                column: "AsesorAnteriorId");

            migrationBuilder.CreateIndex(
                name: "IX_EXPEDIENTE_DETALLES_AsesorNuevorId",
                table: "EXPEDIENTE_DETALLES",
                column: "AsesorNuevorId");

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

            migrationBuilder.CreateIndex(
                name: "IX_EXPEDIENTE_DETALLES_ExpedienteId",
                table: "EXPEDIENTE_DETALLES",
                column: "ExpedienteId");

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

            migrationBuilder.CreateIndex(
                name: "IX_EXPEDIENTES_Activo",
                table: "EXPEDIENTES",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_EXPEDIENTES_AsesorId",
                table: "EXPEDIENTES",
                column: "AsesorId");

            migrationBuilder.CreateIndex(
                name: "IX_EXPEDIENTES_Codigo",
                table: "EXPEDIENTES",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EXPEDIENTES_Estatus",
                table: "EXPEDIENTES",
                column: "Estatus");

            migrationBuilder.CreateIndex(
                name: "IX_EXPEDIENTES_EtapaDetalleId",
                table: "EXPEDIENTES",
                column: "EtapaDetalleId");

            migrationBuilder.CreateIndex(
                name: "IX_EXPEDIENTES_EtapaId",
                table: "EXPEDIENTES",
                column: "EtapaId");

            migrationBuilder.CreateIndex(
                name: "IX_EXPEDIENTES_ExpedienteRelacionadoId",
                table: "EXPEDIENTES",
                column: "ExpedienteRelacionadoId");

            migrationBuilder.CreateIndex(
                name: "IX_EXPEDIENTES_FechaIngreso",
                table: "EXPEDIENTES",
                column: "FechaIngreso");

            migrationBuilder.CreateIndex(
                name: "IX_EXPEDIENTES_RemitenteId",
                table: "EXPEDIENTES",
                column: "RemitenteId");

            migrationBuilder.CreateIndex(
                name: "IX_FLUJOS_Activo",
                table: "FLUJOS",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_FLUJOS_Nombre",
                table: "FLUJOS",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_REMITENTE_Activo",
                table: "REMITENTE",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_USUARIOS_Activo",
                table: "USUARIOS",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_USUARIOS_Username",
                table: "USUARIOS",
                column: "Username");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CAMPOS");

            migrationBuilder.DropTable(
                name: "EXPEDIENTE_DETALLES");

            migrationBuilder.DropTable(
                name: "EXPEDIENTE_NOTAS");

            migrationBuilder.DropTable(
                name: "EXPEDIENTES");

            migrationBuilder.DropTable(
                name: "ETAPA_DETALLES");

            migrationBuilder.DropTable(
                name: "REMITENTE");

            migrationBuilder.DropTable(
                name: "USUARIOS");

            migrationBuilder.DropTable(
                name: "ETAPAS");

            migrationBuilder.DropTable(
                name: "FLUJOS");
        }
    }
}
