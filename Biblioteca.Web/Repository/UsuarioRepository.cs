using Biblioteca.Web.Context;
using Biblioteca.Web.Models;
using Biblioteca.Web.Repository.Interface;
using Microsoft.EntityFrameworkCore;
//using Microsoft.JSInterop.Infrastructure;

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
                usuario.DataAtualizacao = DateTime.UtcNow;
                _context.Usuarios.Update(usuario);         
                await _context.SaveChangesAsync();         
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





        public async Task<bool>ExcluirUsuarioAsync(string usuarioId)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == usuarioId);

            if (usuario == null)
                return false;

            var livrosDoUsuario = await _context.Livros
                .Where(l => l.UsuarioId == usuarioId)
                .ToListAsync();

            // Excluir imagens dos livros
            foreach (var livro in livrosDoUsuario)
            {
                if (!string.IsNullOrEmpty(livro.Imagem))
                {
                    string caminhoImagem = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", livro.Imagem);

                    if (System.IO.File.Exists(caminhoImagem))
                    {
                        System.IO.File.Delete(caminhoImagem);
                    }
                }
            }

            _context.Livros.RemoveRange(livrosDoUsuario);
            _context.Usuarios.Remove(usuario);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> ExcluirUsuariosInativosAsync()
        {
            var doisMesesAtras = DateTime.UtcNow.AddMonths(-2);

            var usuariosInativos = await _context.Usuarios
                .Where(u => u.DataAtualizacao != null && u.DataAtualizacao <= doisMesesAtras)
                .ToListAsync();

            foreach (var usuario in usuariosInativos)
            {
                var livrosDoUsuario = await _context.Livros
                    .Where(l => l.UsuarioId == usuario.Id)
                    .ToListAsync();

                foreach (var livro in livrosDoUsuario)
                {
                    if (!string.IsNullOrEmpty(livro.Imagem))
                    {
                        string caminhoImagem = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", livro.Imagem);

                        if (System.IO.File.Exists(caminhoImagem))
                        {
                            System.IO.File.Delete(caminhoImagem);
                        }
                    }
                }

                _context.Livros.RemoveRange(livrosDoUsuario);
                _context.Usuarios.Remove(usuario);
            }

            return await _context.SaveChangesAsync();
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

        public async Task<string> CriptografarSenha(string novaSenha)
        {
            return await Task.Run(() => BCrypt.Net.BCrypt.HashPassword(novaSenha)); //BCrypt.Net.BCrypt.HashPassword(novaSenha);
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
