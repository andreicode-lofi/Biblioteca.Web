using Biblioteca.Web.Context;
using Biblioteca.Web.Models;
using Biblioteca.Web.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Web.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UsuarioModel?> LoginAsync(string email, string senha)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

            if (usuario != null && BCrypt.Net.BCrypt.Verify(senha, usuario.SenhaHas))
            {
                return usuario;
            }

            return null;
        }

        public async Task<bool> RegistrarUsuarioAsync(string nome, string email, string senha)
        {
            if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
                return false;

            if (await _context.Usuarios.AnyAsync(u => u.Email == email))
                return false; // Já existe um usuário com esse email

            var novoUsuario = new UsuarioModel
            {
                Id = Guid.NewGuid().ToString(),
                Nome = nome,
                Email = email,
                SenhaHas = BCrypt.Net.BCrypt.HashPassword(senha),
                DataRegistro = DateTime.UtcNow
            };

            _context.Usuarios.Add(novoUsuario);
            await _context.SaveChangesAsync();
            return true;
        }

        //===================================redefinição de senha==================================================================

        public async Task<string?> GerarTokenRedefinicaoAsync(string email)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

            if (usuario == null)
                return null;

            usuario.TokenRedefinicao = Guid.NewGuid().ToString();
            usuario.TokenExperiracao = DateTime.UtcNow.AddMinutes(30);

            await _context.SaveChangesAsync();
            return usuario.TokenRedefinicao;
        }

        public string CriptografarSenha(string senha)
        {
            return BCrypt.Net.BCrypt.HashPassword(senha);
        }

        public async Task<UsuarioModel?> BuscarPorTokenAsync(string token)
        {
            return await _context.Usuarios
            .FirstOrDefaultAsync(u => u.TokenRedefinicao == token && u.TokenExperiracao > DateTime.UtcNow);
        }

        public async Task<bool> AtualizarUsuarioSenhaAsync(UsuarioModel usuarioAtualizado)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == usuarioAtualizado.Id);

            if (usuario == null)
                return false;

            usuario.SenhaHas = usuarioAtualizado.SenhaHas;
            usuario.TokenRedefinicao = null;
            usuario.TokenExperiracao = null;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
