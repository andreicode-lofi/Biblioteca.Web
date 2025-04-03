using Biblioteca.Servico.model;
using Biblioteca.Servico.Servicos;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace Biblioteca.Web.Controllers;

public class LivroController : Controller
{
    private readonly ILogger<LivroController> _logger;
    private readonly GerenciadorDelivros _gerenciadorDelivros;
    private readonly string _caminhoImagem;

    public LivroController(ILogger<LivroController> logger, GerenciadorDelivros gerenciadorDelivros)
    {
        _logger = logger;
        _gerenciadorDelivros = gerenciadorDelivros;
    }

    [HttpGet]
    public IActionResult Index(int? page, string pesquisa)
    {
        int pageSize = 6;
        int pageNumber = page ?? 1;

        var livros = _gerenciadorDelivros.getAll();

        if (!string.IsNullOrEmpty(pesquisa))
        {
            pesquisa = pesquisa.ToLower();

            livros = livros.Where(l =>
                l.Name.ToLower().Contains(pesquisa) ||
                l.Autor.ToLower().Contains(pesquisa) ||
                l.Genero.ToLower().Contains(pesquisa)
            ).ToList();
        }

        ViewData["ConsultaPesquisa"] = pesquisa;

        var livrosPaginados = livros.OrderBy(livros => livros.Name).ToPagedList(pageNumber, pageSize);
        return View(livrosPaginados);
    }

    [HttpGet("Livro/Create")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost("Livro/Create")]
    public async Task<IActionResult> Create(LivroModel model, IFormFile foto)
    {
        if (foto != null)
        {
            var caminhoImagem = await GeradorImagemAsync(foto);

            var livro = new LivroModel
            {
                Name = model.Name,
                Autor = model.Autor,
                Genero = model.Genero,
                Idioma = model.Idioma,
                LivroFinalizado = model.LivroFinalizado,
                Imagem = caminhoImagem,
                ano = model.ano,
                Sinopese = model.Sinopese,
                Comentarios = model.Comentarios,
                Avaliacao = model.Avaliacao,
                NumeroPaginas = model.NumeroPaginas,
                TrechosFavoritos = model.TrechosFavoritos
            };

            await _gerenciadorDelivros.addLivroAsync(livro);
            return RedirectToAction("Index");
        }
        return View();
    }

    public async Task<string> GeradorImagemAsync(IFormFile foto)
    {
        var codigoUnico = Guid.NewGuid().ToString();
        var nomeCaminho = foto.FileName.Replace(" ", "").ToLower() + codigoUnico + ".png";
        //criando pasta imagem em wwwroot
        string caminhoSalvarImagem = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

        //Se não existi a pasta images, entre no bloco if e crie uma 
        if (!Directory.Exists(caminhoSalvarImagem))
        {
            Directory.CreateDirectory(caminhoSalvarImagem);
        }

        string caminhoCompletoImagem = Path.Combine(caminhoSalvarImagem, nomeCaminho);

        //criando um aquivo dentro de uma string, e salvando
        using (var stream = System.IO.File.Create(caminhoCompletoImagem))
        {
            await foto.CopyToAsync(stream);
        }

        return nomeCaminho;
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(string id)
    {
        var livroOriginal = await _gerenciadorDelivros.getByIdAsync(id);

        if (livroOriginal != null && !string.IsNullOrEmpty(livroOriginal.Imagem))
        {
            // construir o caminho da imagem
            string caminhoImagem = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", livroOriginal.Imagem);

            //Verificar se o arquivo existe
            if (System.IO.File.Exists(caminhoImagem))
            {
                System.IO.File.Delete(caminhoImagem);
            }
        }
        await _gerenciadorDelivros.RemoveAsync(id);

        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Details(string id)
    {
        var livro = await _gerenciadorDelivros.getByIdAsync(id);
        return View(livro);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var livro = await _gerenciadorDelivros.getByIdAsync(id);
        return View(livro);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(string id, LivroModel livro, IFormFile? foto, List<string> trechosFavoritos)
    {
        var livroOriginal = await _gerenciadorDelivros.getByIdAsync(id);

        if (livroOriginal == null)
        {
            TempData["Erro"] = "Livro não encontrado!";
            return RedirectToAction("Index");
        }
        //---------------------------------------------------
        if (foto != null && foto.Length > 0)
        {
            // Remove a imagem antiga antes de salvar a nova
            if (!string.IsNullOrEmpty(livroOriginal.Imagem))
            {
                // construir o caminho da imagem
                string caminhoAntigo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", livroOriginal.Imagem.TrimStart('/'));

                if (System.IO.File.Exists(caminhoAntigo))
                {
                    System.IO.File.Delete(caminhoAntigo);
                }

                // Salva a nova imagem e atualiza o caminho
                string caminhoImagem = await GeradorImagemAsync(foto);
                livro.Imagem = $"/images/{caminhoImagem}";
            }
        }
        else
        {
            livro.Imagem = livroOriginal.Imagem;
        }

        // Atualização dos trechos favoritos
        //---------------------------------------------------
        if (trechosFavoritos != null)
        {
            livro.TrechosFavoritos = trechosFavoritos.Where(t => !string.IsNullOrWhiteSpace(t)).ToList();
        }
        else
        {
            livro.TrechosFavoritos = new List<string>();
        }

        //atualiza
        await _gerenciadorDelivros.updateAsync(id, livro);
        return RedirectToAction("Index");
    }
}
