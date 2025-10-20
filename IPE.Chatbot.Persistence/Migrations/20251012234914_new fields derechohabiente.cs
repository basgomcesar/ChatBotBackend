using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IPE.Chatbot.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class newfieldsderechohabiente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DerechohabienteEntity",
                table: "DerechohabienteEntity");

            migrationBuilder.RenameTable(
                name: "DerechohabienteEntity",
                newName: "Derechohabientes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Derechohabientes",
                table: "Derechohabientes",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Derechohabientes",
                table: "Derechohabientes");

            migrationBuilder.RenameTable(
                name: "Derechohabientes",
                newName: "DerechohabienteEntity");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DerechohabienteEntity",
                table: "DerechohabienteEntity",
                column: "Id");
        }
    }
}
