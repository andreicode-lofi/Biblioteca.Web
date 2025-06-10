using Biblioteca.Web.Models;
using Biblioteca.Web.Repository.Interface;
using System.Text.Json;

public class GoogleBooksService : IGoogleBooksService
{
    private readonly HttpClient _httpClient;

    public GoogleBooksService()
    {
        _httpClient = new HttpClient();
    }

    public async Task<LivroModel> BuscarDadosDoLivroAsync(string nomeLivro)
    {
        if (string.IsNullOrWhiteSpace(nomeLivro)) return null;

        var url = $"https://www.googleapis.com/books/v1/volumes?q=intitle:{Uri.EscapeDataString(nomeLivro)}";

        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync();

        var resultado = JsonSerializer.Deserialize<GoogleBooksResponse>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        var item = resultado?.Items?.FirstOrDefault()?.VolumeInfo;
        if (item == null) return null;

        return new LivroModel
        {
            Name = item.Title,
            Autor = item.Authors?.FirstOrDefault(),
            Genero = item.Categories?.FirstOrDefault(),
            ano = ExtrairAno(item.PublishedDate),
            Sinopese = item.Description,
            NumeroPaginas = item.PageCount?.ToString(),
            Imagem = item.ImageLinks?.Thumbnail
        };
    }

    private int? ExtrairAno(string publishedDate)
    {
        if (string.IsNullOrWhiteSpace(publishedDate)) return null;
        if (int.TryParse(publishedDate.Split('-')[0], out int ano))
            return ano;
        return null;
    }
}