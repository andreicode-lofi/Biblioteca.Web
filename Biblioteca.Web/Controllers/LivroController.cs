using Biblioteca.Servico.model;
using Biblioteca.Servico.Servicos;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace Biblioteca.Web.Controllers;

public class LivroController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly GerenciadorDelivros _gerenciadorDelivros;
    private readonly string _caminhoImagem;

    public LivroController(ILogger<HomeController> logger, GerenciadorDelivros gerenciadorDelivros)
    {
        _logger = logger;
        _gerenciadorDelivros = gerenciadorDelivros;
    }

    [HttpGet]
    public IActionResult Index(int? page)
    {
        int pageSize = 6;
        int pageNumber = (page ?? 1);

        var livros = _gerenciadorDelivros.getAll()
        .OrderBy(l => l.Name)
        .ToPagedList(pageNumber, pageSize);

        return View(livros);

        /* var livros = _gerenciadorDelivros.getAll();
         return View(livros);*/
    }

    [HttpGet("Livro/Create")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost("Livro/Create")]
    public IActionResult Create(LivroModel model, IFormFile foto)
    {
        if (foto != null)
        {
            var caminhoImagem = GeradorImagem(foto);

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

            _gerenciadorDelivros.AddLivro(livro);
            return RedirectToAction("Index");
        }
        return View();
    }

    public string GeradorImagem(IFormFile foto)
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
            foto.CopyTo(stream);
        }

        return nomeCaminho;
    }

    [HttpDelete]
    public IActionResult Delete(string id)
    {
        _gerenciadorDelivros.Remove(id);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Edit(string id)
    {
        var livro = _gerenciadorDelivros.GetById(id);
        return View(livro);
    }

    [HttpPost]
    public IActionResult Edit(string id, LivroModel livro, IFormFile? foto)
    {
        if (foto != null && foto.Length > 0)
        {
            var caminhoImagem = GeradorImagem(foto);
            livro.Imagem = caminhoImagem;
        }

        _gerenciadorDelivros.Atualizar(id, livro);
        return RedirectToAction("Index");
    }
}
