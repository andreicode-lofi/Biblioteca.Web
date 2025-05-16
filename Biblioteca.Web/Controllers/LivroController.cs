using Biblioteca.Servico.model;
using Biblioteca.Servico.Servicos;
using Biblioteca.Web.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace Biblioteca.Web.Controllers;

public class LivroController : Controller
{
    private readonly ILogger<LivroController> _logger;
    private readonly GerenciadorDelivros _gerenciadorDelivros;
    private readonly ILivroRepository _ilivroRepository;
    private readonly string _caminhoImagem;

    public LivroController(ILogger<LivroController> logger, GerenciadorDelivros gerenciadorDelivros, ILivroRepository livroRepository)
    {
        _logger = logger;
        _gerenciadorDelivros = gerenciadorDelivros;
        _ilivroRepository = livroRepository;
    }

    [HttpGet]
    public IActionResult Index(int? page, string pesquisa)
    {
        var usuarioId = HttpContext.Session.GetString("UsuarioId");
        if(usuarioId == null)
            return RedirectToAction("Login", "Usuario");

        int pageSize = 6;
        int pageNumber = page ?? 1;

        var livros = _ilivroRepository.GetAll(usuarioId); //_gerenciadorDelivros.getAll(usuarioId);

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

        var usuarioId = HttpContext.Session.GetString("UsuarioId");
        if (usuarioId == null) return RedirectToAction("Login", "Index");

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

            await _gerenciadorDelivros.addLivroAsync(livro, usuarioId);
            return RedirectToAction("Index");
        }
        return View();
    }

    public async Task<string> GeradorImagemAsync(IFormFile foto)
    {
        var codigoUnico = Guid.NewGuid().ToString();
        var nomeCaminho = foto.FileName.Replace(" ", "").ToLower() + codigoUnico + ".png";

        string caminhoSalvarImagem = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

        if (!Directory.Exists(caminhoSalvarImagem))
        {
            Directory.CreateDirectory(caminhoSalvarImagem);
        }

        string caminhoCompletoImagem = Path.Combine(caminhoSalvarImagem, nomeCaminho);

        using (var stream = System.IO.File.Create(caminhoCompletoImagem))
        {
            await foto.CopyToAsync(stream);
        }

        return nomeCaminho;
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(string id)
    {
        var usuarioId = HttpContext.Session.GetString("UsuarioId");

        var livroOriginal = await _gerenciadorDelivros.getByIdAsync(id, usuarioId);

        if (livroOriginal == null)
            return Unauthorized();

        if (livroOriginal != null && !string.IsNullOrEmpty(livroOriginal.Imagem))
        {
            string caminhoImagem = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", livroOriginal.Imagem);

            //Verificar se o arquivo existe
            if (System.IO.File.Exists(caminhoImagem))
            {
                System.IO.File.Delete(caminhoImagem);
            }
        }
        await _gerenciadorDelivros.RemoveAsync(id, usuarioId);

        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Details(string id)
    {
        var usuarioId = HttpContext.Session.GetString("UsuarioId");
        var livro = await _gerenciadorDelivros.getByIdAsync(id, usuarioId);

        if (livro == null)
            return Unauthorized();

        return View(livro);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var usuarioId = HttpContext.Session.GetString("UsuarioId");
        
        var livro = await _gerenciadorDelivros.getByIdAsync(id, usuarioId);

        if (livro == null)
            return Unauthorized();

        return View(livro);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(string id, LivroModel livro, IFormFile? foto, List<string> trechosFavoritos)
    {
        var usuarioId = HttpContext.Session.GetString("UsuarioId");

        var livroOriginal = await _gerenciadorDelivros.getByIdAsync(id, usuarioId);

        if (livroOriginal == null)
        {
            TempData["Erro"] = "Livro não encontrado!";
            return RedirectToAction("Index");
        }
        //---------------------------------------------------
        if (foto != null && foto.Length > 0)
        {
            if (!string.IsNullOrEmpty(livroOriginal.Imagem))
            {
                string caminhoAntigo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", livroOriginal.Imagem.TrimStart('/'));

                if (System.IO.File.Exists(caminhoAntigo))
                {
                    System.IO.File.Delete(caminhoAntigo);
                }
            }

            string caminhoImagem = await GeradorImagemAsync(foto);
            livro.Imagem = caminhoImagem;
        }
        else
        {
            livro.Imagem = Path.GetFileName(livroOriginal.Imagem);
        }

        // Atualização trechos favoritos
        //---------------------------------------------------
        if (trechosFavoritos != null)
        {
            livro.TrechosFavoritos = trechosFavoritos.Where(t => !string.IsNullOrWhiteSpace(t)).ToList();
        }
        else
        {
            livro.TrechosFavoritos = new List<string>();
        }

        livro.Id = livroOriginal.Id;
        livro.UsuarioId = usuarioId;
        livro.TrechosFavoritos = trechosFavoritos;

        //atualiza
        await _gerenciadorDelivros.updateAsync(id, livro, usuarioId);
        return RedirectToAction("Index");
    }
}
