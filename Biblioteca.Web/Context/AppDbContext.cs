using Biblioteca.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace Biblioteca.Web.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<LivroModel> LivroModel {get; set;}
        public DbSet<UsuarioModel> UsuarioModel { get; set; }
        public DbSet<RedefinirSenhaModel> RedefinirSenhasModel { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Diz ao EF Core que esse modelo NÃO tem chave e NÃO deve virar tabela
            modelBuilder.Entity<RedefinirSenhaModel>().HasNoKey();




            // Conversor de List<string> para JSON (TrechosFavoritos)
            var trechosConverter = new ValueConverter<List<string>, string>(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null), // evita uso de sobrecarga com argumento opcional
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
            );

            modelBuilder.Entity<LivroModel>()
                .Property(l => l.TrechosFavoritos)
                .HasConversion(trechosConverter);

        }

    }
}
