using Biblioteca.Servico.model;

namespace Biblioteca.Servico.model;

public class UsuarioModel
{
    public string? Id { get; set; }
    public string? Nome { get; set; }
    public string? Email { get; set; }
    public string? Senha { get; set; }
    //public List<LivroModel>? Livros { get; set; } = new List<LivroModel>();// Lista de livros do usuário
}