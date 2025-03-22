using System.Text.Json;
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
    public async Task addLivroAsync(LivroModel livro)
    {
        _livros?.Add(livro);
        await saveLivroAsync();
    }
    public List<LivroModel> getAll()
    {
        return _livros;
    }
    public async Task RemoveAsync(string id)
    {
        _livros?.RemoveAll(l => l.Id == id);
        await saveLivroAsync();
    }
    public async Task<LivroModel?> getByIdAsync(string id)
    {
        await carregarLivrosAsync(); // Recarrega os dados do JSON antes de buscar
        return _livros?.FirstOrDefault(l => l.Id == id);
    }
    public async Task updateAsync(string id, LivroModel livro)
    {
        var index = _livros.FindIndex(l => l.Id == id);
        if (index == -1)
        {
            throw new InvalidOperationException("Livros não encontrado");
        }
        _livros[index] = livro;
        await saveLivroAsync();
    }
}





