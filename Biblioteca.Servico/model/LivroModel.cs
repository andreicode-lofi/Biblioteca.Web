namespace Biblioteca.Servico.model;

public class LivroModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Autor { get; set; } = string.Empty;
    public string Genero { get; set; } = string.Empty;
    public string Idioma { get; set; } = string.Empty;
    public bool LivroFinalizado { get; set; }
    public string Imagem { get; set; } = string.Empty;
    public int ano { get; set; }


    /*-----------------detail livro------------------------*/
    public string Sinopese { get; set; } = string.Empty; //Um pequeno resumo do livro
    public string Comentarios { get; set; } = string.Empty;
    public string Avaliacao { get; set; } = string.Empty;
    public string NumeroPaginas { get; set; } = string.Empty;
    // Lista para armazenar trechos favoritos do livro
    public List<string> TrechosFavoritos { get; set; } = new List<string>();
}