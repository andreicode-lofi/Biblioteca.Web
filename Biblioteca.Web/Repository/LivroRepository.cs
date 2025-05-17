using System.Globalization;
using System.Text;
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

            _context.Livros.Add(livro);
            await _context.SaveChangesAsync();
        }

        public List<LivroModel> GetAll(string usuarioId)
        {
            var livros = _context.Livros
                .Where(l => l.UsuarioId == usuarioId)
                .ToList();

            return livros ?? new List<LivroModel>();
        }

        public async Task<LivroModel?> GetByIdAsync(string id, string usuarioId)
        {
            return await _context.Livros
                .FirstOrDefaultAsync(l => l.Id == id && l.UsuarioId == usuarioId);
        }

        public async Task RemoveAsync(string id, string usuarioId)
        {
            var livro = await _context.Livros
                .FirstOrDefaultAsync(l => l.Id == id && l.UsuarioId == usuarioId);
            if (livro != null)
            {
                _context.Livros.Remove(livro);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(string id, LivroModel livro, string usuarioId)
        {
            var livroExistente = await _context.Livros
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

        public string RemoverAcentos(string pesquisa)
        {
            if (string.IsNullOrEmpty(pesquisa)) return string.Empty;

            var normalized = pesquisa.Normalize(System.Text.NormalizationForm.FormD);
            var builder = new StringBuilder();

            foreach (var item in pesquisa)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(item);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                    builder.Append(item);
            }

            return builder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
