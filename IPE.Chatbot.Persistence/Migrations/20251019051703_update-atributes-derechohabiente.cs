using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IPE.Chatbot.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class updateatributesderechohabiente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Afiliacion",
                table: "Derechohabientes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "Derechohabientes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UltimaInteraccion",
                table: "Derechohabientes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Afiliacion",
                table: "Derechohabientes");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "Derechohabientes");

            migrationBuilder.DropColumn(
                name: "UltimaInteraccion",
                table: "Derechohabientes");
        }
    }
}
