using System.Text.Json;
using Biblioteca.Servico.model;
using BCrypt.Net;
using Microsoft.AspNetCore.Http;





namespace Biblioteca.Servico.Servicos;

public class GerenciadorDeUsuarios
{
    private readonly string _caminhoDoArquivo = "Usuarios.json";

    private List<UsuarioModel> _usuarios = new List<UsuarioModel>();

    public GerenciadorDeUsuarios()
    {
        carregarUsuariosAsync().Wait();
    }
    private async Task carregarUsuariosAsync()
    {
        if (File.Exists(_caminhoDoArquivo))
        {
            var json = await File.ReadAllTextAsync(_caminhoDoArquivo);
            _usuarios = JsonSerializer.Deserialize<List<UsuarioModel>>(json) ?? new List<UsuarioModel>();
        }
        else
        {
            _usuarios = new List<UsuarioModel>();
        }
    }
    private async Task salvarUsuariosAsync()
    {
        var json = JsonSerializer.Serialize(_usuarios, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_caminhoDoArquivo, json);
    }

    public async Task<UsuarioModel?> loginAsync(string email, string senha)
    {
        await carregarUsuariosAsync();
        var usuario = _usuarios.FirstOrDefault(u => u.Email == email);

        if (usuario != null && BCrypt.Net.BCrypt.Verify(senha, usuario.SenhaHas))
        {
            return usuario;
        }

        return null;
    }

    public async Task<bool> registrarUsuarioAsync(string nome, string email, string senha)
    {

        await carregarUsuariosAsync();

        if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
        {
            return false;// E-mail e senha são obrigatórios
        }

        if (_usuarios.Any(u => u.Email == email))
        {
            return false;// Já existe um usuário com este e-mail
        }

        var novoUsuario = new UsuarioModel
        {
            Id = Guid.NewGuid().ToString(),
            Nome = nome,
            Email = email,
            SenhaHas = BCrypt.Net.BCrypt.HashPassword(senha),//No futuro, criptografar essa senha
            DataRegistro = DateTime.UtcNow
        };

        _usuarios.Add(novoUsuario);
        await salvarUsuariosAsync();
        return true;
    }

    public async Task<UsuarioModel?> getByIdAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return null;
        }

        await carregarUsuariosAsync();

        var usuario = _usuarios.FirstOrDefault(u => u.Id == id);

        return usuario ?? null;
    }

    public async Task<bool> updateAsync(string id, string nome, string email, string senhaHas)
    {
        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(email))
        {
            return false;
        }

        await carregarUsuariosAsync();

        var usuario = await getByIdAsync(id);

        usuario.Nome = nome;
        usuario.Email = email;

        if (!string.IsNullOrEmpty(senhaHas))
        {
            usuario.SenhaHas = BCrypt.Net.BCrypt.HashPassword(senhaHas); // Atualiza a senha somente se for fornecida
        }

        usuario.DataAtualizacao = DateTime.UtcNow;

        await salvarUsuariosAsync();
        return true;
    }

    public async Task<bool> Delete(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return false;
        }

        await carregarUsuariosAsync();

        var usuario = await getByIdAsync(id);

        if (usuario == null)
        {
            return false; // Usuário não encontrado
        }

        _usuarios.Remove(usuario);

        await salvarUsuariosAsync();

        return true;
    }
}