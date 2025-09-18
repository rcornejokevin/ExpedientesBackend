using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DBHandler.Migrations.DBHandler
{
    /// <inheritdoc />
    public partial class NuevosCampos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Operativo",
                table: "USUARIOS",
                type: "BOOLEAN",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RegresoARemitente",
                table: "FLUJOS",
                type: "BOOLEAN",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Operativo",
                table: "USUARIOS");

            migrationBuilder.DropColumn(
                name: "RegresoARemitente",
                table: "FLUJOS");
        }
    }
}
