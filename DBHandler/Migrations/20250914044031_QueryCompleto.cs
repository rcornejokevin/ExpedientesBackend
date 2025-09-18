using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DBHandler.Migrations
{
    /// <inheritdoc />
    public partial class QueryCompleto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "USUARIOSNIDA",
                columns: table => new
                {
                    IDUSUARIO = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOMBREUSUARIO = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    IDROL = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    IDSISTEMA = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    NIT = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CLAVE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    NOMBRE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    FECHACREACION = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    CAMBIARCLAVE = table.Column<bool>(type: "BOOLEAN", nullable: true),
                    BLOQUEADO = table.Column<bool>(type: "BOOLEAN", nullable: true),
                    FIRMA = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ULTIMO_CAMBIO_CLAVE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    FIRMA_ELECTRONICA = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    DIRECTOR_GESTION = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    USERNAME_FEA = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PASSWORD_FEA = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CLAVE_FEA = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PIN_FEA = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    VERSION_FEA = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    USUARIO_FEA = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CODIGO_VERIFICACION = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ESTADO_CODIGO = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USUARIOSNIDA", x => x.IDUSUARIO);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "USUARIOSNIDA");
        }
    }
}
