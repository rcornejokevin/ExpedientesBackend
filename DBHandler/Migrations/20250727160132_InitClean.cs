using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DBHandler.Migrations
{
    /// <inheritdoc />
    public partial class InitClean : Migration
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
                    Detalle = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Activo = table.Column<bool>(type: "BOOLEAN", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FLUJOS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "USUARIOS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Username = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Perfil = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Activo = table.Column<bool>(type: "BOOLEAN", nullable: false)
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
                    Activo = table.Column<bool>(type: "BOOLEAN", nullable: false),
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
                name: "ETAPA_DETALLES",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nombre = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Orden = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Activo = table.Column<bool>(type: "BOOLEAN", nullable: false),
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
                name: "CAMPOS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nombre = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Orden = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Tipo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Activo = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    Requerido = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    EtapaId = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    EtapaDetalleId = table.Column<int>(type: "NUMBER(10)", nullable: true)
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
                        name: "FK_CAMPOS_ETAPA_DETALLES_EtapaDetalleId",
                        column: x => x.EtapaDetalleId,
                        principalTable: "ETAPA_DETALLES",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EXPEDIENTES",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Codigo = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Estatus = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    FechaIngreso = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    EtapaId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    EtapaDetalleId = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    Activo = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    Ubicacion = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
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
                });

            migrationBuilder.CreateTable(
                name: "EXPEDIENTE_DETALLES",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Fecha = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ExpedienteId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Responsable = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EXPEDIENTE_DETALLES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EXPEDIENTE_DETALLES_EXPEDIENTES_ExpedienteId",
                        column: x => x.ExpedienteId,
                        principalTable: "EXPEDIENTES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CAMPOS_Activo",
                table: "CAMPOS",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_CAMPOS_EtapaDetalleId",
                table: "CAMPOS",
                column: "EtapaDetalleId");

            migrationBuilder.CreateIndex(
                name: "IX_CAMPOS_EtapaId",
                table: "CAMPOS",
                column: "EtapaId");

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
                name: "IX_EXPEDIENTE_DETALLES_ExpedienteId",
                table: "EXPEDIENTE_DETALLES",
                column: "ExpedienteId");

            migrationBuilder.CreateIndex(
                name: "IX_EXPEDIENTES_Activo",
                table: "EXPEDIENTES",
                column: "Activo");

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
                name: "IX_EXPEDIENTES_FechaIngreso",
                table: "EXPEDIENTES",
                column: "FechaIngreso");

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
                name: "USUARIOS");

            migrationBuilder.DropTable(
                name: "EXPEDIENTES");

            migrationBuilder.DropTable(
                name: "ETAPA_DETALLES");

            migrationBuilder.DropTable(
                name: "ETAPAS");

            migrationBuilder.DropTable(
                name: "FLUJOS");
        }
    }
}
