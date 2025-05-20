using Biblioteca.Web.Context.EfConfigurations;
using Biblioteca.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace Biblioteca.Web.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<LivroModel> Livros {get; set;}
        public DbSet<UsuarioModel> Usuarios { get; set; }
        public DbSet<RedefinirSenhaModel> RedefinirSenhasModel { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UsuarioModel>().ToTable("UsuarioModel");//mudar nome:UssuarioModel para usuarios

            // Diz ao EF Core que esse modelo NÃO tem chave e NÃO deve virar tabela
            modelBuilder.Entity<RedefinirSenhaModel>().HasNoKey();

            //Instanciando configuração ef.core para entidade Livro
            modelBuilder.ApplyConfiguration(new LivroConfiguration());

        }
    }
}
