using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Biblioteca.Web.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LivroModel",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Autor = table.Column<string>(type: "text", nullable: true),
                    Genero = table.Column<string>(type: "text", nullable: true),
                    Idioma = table.Column<string>(type: "text", nullable: true),
                    LivroFinalizado = table.Column<bool>(type: "boolean", nullable: false),
                    Imagem = table.Column<string>(type: "text", nullable: true),
                    ano = table.Column<int>(type: "integer", nullable: true),
                    Sinopese = table.Column<string>(type: "text", nullable: true),
                    Comentarios = table.Column<string>(type: "text", nullable: true),
                    Avaliacao = table.Column<string>(type: "text", nullable: true),
                    NumeroPaginas = table.Column<string>(type: "text", nullable: true),
                    UsuarioId = table.Column<string>(type: "text", nullable: true),
                    Favorito = table.Column<bool>(type: "boolean", nullable: false),
                    TrechosFavoritos = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LivroModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RedefinirSenhasModel",
                columns: table => new
                {
                    Email = table.Column<string>(type: "text", nullable: true),
                    NovaSenha = table.Column<string>(type: "text", nullable: true),
                    ConfirmarSenha = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "UsuarioModel",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    SenhaHas = table.Column<string>(type: "text", nullable: true),
                    DataRegistro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TokenRedefinicao = table.Column<string>(type: "text", nullable: true),
                    TokenExperiracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioModel", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LivroModel");

            migrationBuilder.DropTable(
                name: "RedefinirSenhasModel");

            migrationBuilder.DropTable(
                name: "UsuarioModel");
        }
    }
}
