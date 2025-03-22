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
                ano = model.ano
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
        await _gerenciadorDelivros.RemoveAsync(id);
        return RedirectToAction("Index");
    }
    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var livro = await _gerenciadorDelivros.getByIdAsync(id);
        return View(livro);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(string id, LivroModel livro, IFormFile? foto)
    {
        if (foto != null && foto.Length > 0)
        {
            var caminhoImagem = await GeradorImagemAsync(foto);
            livro.Imagem = caminhoImagem;
        }

        await _gerenciadorDelivros.updateAsync(id, livro);
        return RedirectToAction("Index");
    }
}
