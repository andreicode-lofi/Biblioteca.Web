namespace Biblioteca.Web.Models
{
    public class GoogleBooksResponse
    {
        public List<GoogleBookItem> Items { get; set; }
    }

    public class GoogleBookItem
    {
        public VolumeInfo VolumeInfo { get; set; }
    }

    public class VolumeInfo
    {
        // Nome do Livro
        public string Title { get; set; }

        // Autor
        public List<string> Authors { get; set; }

        // Gênero
        public List<string> Categories { get; set; }

        // Data de Publicação
        public string PublishedDate { get; set; }

        // Sinopse
        public string Description { get; set; }

        // Número de Páginas
        public int? PageCount { get; set; }

        // Imagem da Capa
        public ImageLinks ImageLinks { get; set; }
    }

    public class ImageLinks
    {
        public string Thumbnail { get; set; }
    }
}