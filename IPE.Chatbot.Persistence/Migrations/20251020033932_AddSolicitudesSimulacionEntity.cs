using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IPE.Chatbot.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSolicitudesSimulacionEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SolicitudesSimulacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DerechohabienteId = table.Column<int>(type: "int", nullable: false),
                    TipoSimulacion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaSolicitud = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesSimulacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitudesSimulacion_Derechohabientes_DerechohabienteId",
                        column: x => x.DerechohabienteId,
                        principalTable: "Derechohabientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesSimulacion_DerechohabienteId",
                table: "SolicitudesSimulacion",
                column: "DerechohabienteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SolicitudesSimulacion");
        }
    }
}
