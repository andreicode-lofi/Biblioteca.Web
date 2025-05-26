using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Biblioteca.Web.Migrations
{
    /// <inheritdoc />
    public partial class livrosfavoritoslink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Favorito",
                table: "LivroModel",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Favorito",
                table: "LivroModel");
        }
    }
}
