using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DBHandler.Migrations
{
    /// <inheritdoc />
    public partial class CampoDeCierre3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EstatusAnterior",
                table: "EXPEDIENTE_DETALLES",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EstatusNuevo",
                table: "EXPEDIENTE_DETALLES",
                type: "NVARCHAR2(2000)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstatusAnterior",
                table: "EXPEDIENTE_DETALLES");

            migrationBuilder.DropColumn(
                name: "EstatusNuevo",
                table: "EXPEDIENTE_DETALLES");
        }
    }
}
