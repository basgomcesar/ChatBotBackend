using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IPE.Chatbot.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddnewPropertiesDerechohabiente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Derechohabientes",
                table: "Derechohabientes");

            migrationBuilder.RenameTable(
                name: "Derechohabientes",
                newName: "DerechohabienteEntity");

            migrationBuilder.RenameColumn(
                name: "Clave",
                table: "DerechohabienteEntity",
                newName: "Telefono");

            migrationBuilder.AddColumn<string>(
                name: "Flujo",
                table: "DerechohabienteEntity",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Folio",
                table: "DerechohabienteEntity",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Paso",
                table: "DerechohabienteEntity",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Tipo",
                table: "DerechohabienteEntity",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DerechohabienteEntity",
                table: "DerechohabienteEntity",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DerechohabienteEntity",
                table: "DerechohabienteEntity");

            migrationBuilder.DropColumn(
                name: "Flujo",
                table: "DerechohabienteEntity");

            migrationBuilder.DropColumn(
                name: "Folio",
                table: "DerechohabienteEntity");

            migrationBuilder.DropColumn(
                name: "Paso",
                table: "DerechohabienteEntity");

            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "DerechohabienteEntity");

            migrationBuilder.RenameTable(
                name: "DerechohabienteEntity",
                newName: "Derechohabientes");

            migrationBuilder.RenameColumn(
                name: "Telefono",
                table: "Derechohabientes",
                newName: "Clave");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Derechohabientes",
                table: "Derechohabientes",
                column: "Id");
        }
    }
}
