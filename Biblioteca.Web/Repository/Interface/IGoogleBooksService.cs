using Biblioteca.Web.Models;

namespace Biblioteca.Web.Repository.Interface
{
    public interface IGoogleBooksService
    {
        Task<LivroModel> BuscarDadosDoLivroAsync(string nomeLivro);
    }
}
