/*using System.Text.Json;
using Biblioteca.Servico.model;

namespace Biblioteca.Servico.Servicos;
public class GerenciadorDelivros
{
    private readonly string _caminhoDoArquivo = "Biblioteca.json";//encapicilamento
    private List<LivroModel>? _livros = new List<LivroModel>();

    public GerenciadorDelivros()
    {
        carregarLivrosAsync().Wait();
    }
    private async Task carregarLivrosAsync()
    {
        if (File.Exists(_caminhoDoArquivo))
        {
            var json = await File.ReadAllTextAsync(_caminhoDoArquivo);
            _livros = JsonSerializer.Deserialize<List<LivroModel>>(json) ?? new List<LivroModel>();
        }
        else
        {
            _livros = new List<LivroModel>(); // Garante que a lista nunca seja null
        }
    }

    private async Task saveLivroAsync()
    {
        var json = JsonSerializer.Serialize(_livros, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_caminhoDoArquivo, json);
    }

    public List<LivroModel> getAll(string usuarioId)
    {

        return _livros.Where(l => l.UsuarioId == usuarioId).ToList();
    }

    public async Task addLivroAsync(LivroModel livro, string usuarioId)
    {
        await carregarLivrosAsync();
        livro.UsuarioId = usuarioId;
        livro.Id ??= Guid.NewGuid().ToString();
        _livros.Add(livro);
        await saveLivroAsync();
    }

    public async Task<LivroModel?> getByIdAsync(string id, string usuarioId)
    {
        await carregarLivrosAsync(); 
        return _livros?.FirstOrDefault(l => l.Id == id && l.UsuarioId == usuarioId);
    }

    public async Task updateAsync(string id, LivroModel livro, string usuarioId)
    {
        await carregarLivrosAsync();
        var index = _livros.FindIndex(l => l.Id == id && l.UsuarioId == usuarioId);
        if (index == -1)
        {
            throw new InvalidOperationException("Livros não encontrado");
        }

        livro.Id = id;
        livro.UsuarioId = usuarioId;

        _livros[index] = livro;
        await saveLivroAsync();
    }

    public async Task RemoveAsync(string id, string usuarioId)
    {
        await carregarLivrosAsync();
        _livros?.RemoveAll(l => l.Id == id && l.UsuarioId == usuarioId);
        await saveLivroAsync();
    }
}

*/



