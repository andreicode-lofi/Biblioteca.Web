using Biblioteca.Web.Context;
using Biblioteca.Web.Models;
using Biblioteca.Web.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Web.Repository
{
    public class LivroRepository : ILivroRepository
    {
        private readonly AppDbContext _context;

        public LivroRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddLivroAsync(LivroModel livro, string usuarioId)
        {
            livro.UsuarioId = usuarioId;

            if (string.IsNullOrEmpty(livro.Id))
                livro.Id = Guid.NewGuid().ToString();

            _context.LivroModel.Add(livro);
            await _context.SaveChangesAsync();
        }

        public async Task<List<LivroModel>> GetAllAsync(string usuarioId)
        {
            return await _context.LivroModel
                .Where(l => l.UsuarioId == usuarioId)
                .ToListAsync();
        }

        public async Task<LivroModel?> GetByIdAsync(string id, string usuarioId)
        {
            return await _context.LivroModel
                .FirstOrDefaultAsync(l => l.Id == id && l.UsuarioId == usuarioId);
        }

        public async Task RemoveAsync(string id, string usuarioId)
        {
            var livro = await _context.LivroModel
                .FirstOrDefaultAsync(l => l.Id == id && l.UsuarioId == usuarioId);
            if(livro != null)
            {
                _context.LivroModel.Remove(livro);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(string id, LivroModel livro, string usuarioId)
        {
            var livroExistente = await _context.LivroModel
               .FirstOrDefaultAsync(l => l.Id == id && l.UsuarioId == usuarioId);

            if (livroExistente == null)
            {
                throw new InvalidOperationException("Livro não encontrado");
            }

            // Atualiza todos os campos
            livroExistente.Name = livro.Name;
            livroExistente.Autor = livro.Autor;
            livroExistente.Genero = livro.Genero;
            livroExistente.Idioma = livro.Idioma;
            livroExistente.LivroFinalizado = livro.LivroFinalizado;
            livroExistente.Imagem = livro.Imagem;
            livroExistente.ano = livro.ano;

            livroExistente.Sinopese = livro.Sinopese;
            livroExistente.Comentarios = livro.Comentarios;
            livroExistente.Avaliacao = livro.Avaliacao;
            livroExistente.NumeroPaginas = livro.NumeroPaginas;

            // Atualiza os trechos favoritos
            livroExistente.TrechosFavoritos = livro.TrechosFavoritos;

            await _context.SaveChangesAsync();
        }
    }
}
