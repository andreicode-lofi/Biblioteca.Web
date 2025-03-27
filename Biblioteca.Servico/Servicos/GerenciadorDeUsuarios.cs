using System.Text.Json;
using Biblioteca.Servico.model;

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
    public async Task<bool> registrarUsuarioAsync(string nome, string email, string senha)
    {
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
            Senha = senha//No futuro, criptografar essa senha
        };

        _usuarios.Add(novoUsuario);
        await salvarUsuariosAsync();
        return true;
    }
    public async Task<UsuarioModel?> loginAsync(string email, string senha)
    {
        await carregarUsuariosAsync();
        return _usuarios.FirstOrDefault(u => u.Email == email && u.Senha == senha);
    }
}