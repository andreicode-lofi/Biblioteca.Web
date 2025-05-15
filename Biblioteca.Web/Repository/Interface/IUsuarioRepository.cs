using Biblioteca.Web.Models;

namespace Biblioteca.Web.Repository.Interface
{
    public interface IUsuarioRepository
    {
        Task<UsuarioModel?> LoginAsync(string email, string senha);
        Task<bool> RegistrarUsuarioAsync(string nome, string email, string senha);
        Task<string?> GerarTokenRedefinicaoAsync(string email);
        Task<UsuarioModel?> BuscarPorTokenAsync(string token);
        Task<bool> AtualizarUsuarioSenhaAsync(UsuarioModel usuarioAtualizado);
        string CriptografarSenha(string senha);
    }
}
