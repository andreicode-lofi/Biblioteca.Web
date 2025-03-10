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
}