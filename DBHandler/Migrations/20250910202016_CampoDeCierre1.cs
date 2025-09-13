using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DBHandler.Migrations
{
    /// <inheritdoc />
    public partial class CampoDeCierre1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "REMITENTE",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Descripcion = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Activo = table.Column<bool>(type: "BOOLEAN", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REMITENTE", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_REMITENTE_Activo",
                table: "REMITENTE",
                column: "Activo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "REMITENTE");
        }
    }
}
