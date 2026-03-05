using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppEnvios.Migrations
{
    /// <inheritdoc />
    public partial class FixRelaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Envios_Clientes_ClienteId",
                table: "Envios");

            migrationBuilder.DropForeignKey(
                name: "FK_Envios_Destinatarios_DestinatarioId",
                table: "Envios");

            migrationBuilder.DropForeignKey(
                name: "FK_Envios_EstadosEnvio_EstadoEnvioEstadoId",
                table: "Envios");

            migrationBuilder.DropIndex(
                name: "IX_Envios_EstadoEnvioEstadoId",
                table: "Envios");

            migrationBuilder.DropColumn(
                name: "EstadoEnvioEstadoId",
                table: "Envios");

            migrationBuilder.CreateIndex(
                name: "IX_Envios_EstadoId",
                table: "Envios",
                column: "EstadoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Envios_Clientes_ClienteId",
                table: "Envios",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "ClienteId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Envios_Destinatarios_DestinatarioId",
                table: "Envios",
                column: "DestinatarioId",
                principalTable: "Destinatarios",
                principalColumn: "DestinatarioId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Envios_EstadosEnvio_EstadoId",
                table: "Envios",
                column: "EstadoId",
                principalTable: "EstadosEnvio",
                principalColumn: "EstadoId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Envios_Clientes_ClienteId",
                table: "Envios");

            migrationBuilder.DropForeignKey(
                name: "FK_Envios_Destinatarios_DestinatarioId",
                table: "Envios");

            migrationBuilder.DropForeignKey(
                name: "FK_Envios_EstadosEnvio_EstadoId",
                table: "Envios");

            migrationBuilder.DropIndex(
                name: "IX_Envios_EstadoId",
                table: "Envios");

            migrationBuilder.AddColumn<int>(
                name: "EstadoEnvioEstadoId",
                table: "Envios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Envios_EstadoEnvioEstadoId",
                table: "Envios",
                column: "EstadoEnvioEstadoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Envios_Clientes_ClienteId",
                table: "Envios",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "ClienteId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Envios_Destinatarios_DestinatarioId",
                table: "Envios",
                column: "DestinatarioId",
                principalTable: "Destinatarios",
                principalColumn: "DestinatarioId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Envios_EstadosEnvio_EstadoEnvioEstadoId",
                table: "Envios",
                column: "EstadoEnvioEstadoId",
                principalTable: "EstadosEnvio",
                principalColumn: "EstadoId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
