using Biblioteca.Web.Models;

namespace Biblioteca.Web.Repository.Interface
{
    public interface ILivroRepository
    {
        Task<List<LivroModel>> GetAllAsync(string usuarioId);
        Task AddLivroAsync(LivroModel livro, string usuarioId);
        Task<LivroModel?> GetByIdAsync(string id, string usuarioId);
        Task UpdateAsync(string id, LivroModel livro, string usuarioId);
        Task RemoveAsync(string id, string usuarioId);
    }
}
