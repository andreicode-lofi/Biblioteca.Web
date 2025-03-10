using System.Text.Json;
using Biblioteca.Servico.model;

namespace Biblioteca.Servico.Servicos;
public class GerenciadorDelivros
{
    private readonly string _caminhoDoArquivo = "Biblioteca.json";//encapicilamento
    private List<LivroModel>? _livros = new List<LivroModel>();

    public GerenciadorDelivros()
    {
        CarregarLivros();
    }

    void CarregarLivros()
    {
        if (File.Exists(_caminhoDoArquivo))
        {
            var json = File.ReadAllText(_caminhoDoArquivo);
            _livros = JsonSerializer.Deserialize<List<LivroModel>>(json) ?? new List<LivroModel>();
        }
        else
        {
            _livros = new List<LivroModel>(); // Garante que a lista nunca seja null
        }
    }

    void SaveLivro()
    {
        var json = JsonSerializer.Serialize(_livros, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_caminhoDoArquivo, json);
    }

    public void AddLivro(LivroModel livro)
    {
        _livros.Add(livro);
        SaveLivro();
    }

    public List<LivroModel> getAll()
    {
        return _livros;
    }
    public void Remove(string id)
    {
        _livros?.RemoveAll(l => l.Id == id);
        SaveLivro();
    }

    public LivroModel? GetById(string id)
    {
        CarregarLivros(); // Recarrega os dados do JSON antes de buscar
        return _livros.FirstOrDefault(l => l.Id == id);
    }

    public void Atualizar(string id, LivroModel livro)
    {
        var index = _livros.FindIndex(l => l.Id == id);
        if (index == -1)
        {
            throw new InvalidOperationException("Livros não encontrado");
        }
        _livros[index] = livro;
        SaveLivro();
    }
}





