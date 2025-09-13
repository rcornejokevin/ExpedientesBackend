using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DBHandler.Migrations
{
    /// <inheritdoc />
    public partial class NewMigration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EXPEDIENTE_DETALLES_USUARIOS_AsesorId",
                table: "EXPEDIENTE_DETALLES");

            migrationBuilder.DropIndex(
                name: "IX_EXPEDIENTE_DETALLES_AsesorId",
                table: "EXPEDIENTE_DETALLES");

            migrationBuilder.RenameColumn(
                name: "AsesorId",
                table: "EXPEDIENTE_DETALLES",
                newName: "AsesorNuevorId");

            migrationBuilder.AlterColumn<int>(
                name: "EtapaAnteriorId",
                table: "EXPEDIENTE_DETALLES",
                type: "NUMBER(10)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "NUMBER(10)");

            migrationBuilder.AddColumn<int>(
                name: "AsesorAnteriorId",
                table: "EXPEDIENTE_DETALLES",
                type: "NUMBER(10)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AsesorAnteriorId",
                table: "EXPEDIENTE_DETALLES");

            migrationBuilder.RenameColumn(
                name: "AsesorNuevorId",
                table: "EXPEDIENTE_DETALLES",
                newName: "AsesorId");

            migrationBuilder.AlterColumn<int>(
                name: "EtapaAnteriorId",
                table: "EXPEDIENTE_DETALLES",
                type: "NUMBER(10)",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "NUMBER(10)",
                oldNullable: true);

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
    }
}
