using Biblioteca.Web.Models;
using Microsoft.EntityFrameworkCore;

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
        }

    }
}
