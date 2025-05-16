using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Biblioteca.Web.Migrations
{
    /// <inheritdoc />
    public partial class RenomearLivroModelParaLivros4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
               name: "TrechosFavoritos",
               table: "LivroModel",
               type: "jsonb",
               nullable: true,
               oldClrType: typeof(string),
               oldType: "text",
               oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
               name: "TrechosFavoritos",
               table: "LivroModel",
               type: "text",
               nullable: true,
               oldClrType: typeof(string),
               oldType: "jsonb",
               oldNullable: true);
        }
    }
}
