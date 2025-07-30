using Biblioteca.Web.Models;
using Biblioteca.Web.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;
using X.PagedList.Extensions;

namespace Biblioteca.Web.Controllers;

public class LivroController : Controller
{
    private readonly ILogger<LivroController> _logger;
    private readonly ILivroRepository _ilivroRepository;
    private readonly IGoogleBooksService _iGoogleBooksService;
    private readonly string? _caminhoImagem;

    public LivroController(ILogger<LivroController> logger, ILivroRepository livroRepository, IGoogleBooksService googleBooksService)
    {
        _logger = logger;
        _ilivroRepository = livroRepository;
        _iGoogleBooksService = googleBooksService;
    }

    [HttpGet]
    public IActionResult Index(int? page, string pesquisa)
    {
        var usuarioId = HttpContext.Session.GetString("UsuarioId");
        if (usuarioId == null)
            return RedirectToAction("Login", "Usuario");

        int pageSize = 6;
        int pageNumber = page ?? 1;

        var livros = _ilivroRepository.GetAll(usuarioId);

        if (!string.IsNullOrEmpty(pesquisa))
        {
            pesquisa = pesquisa.ToLower();

            livros = livros.Where(l =>
                _ilivroRepository.RemoverAcentos(l.Name ?? "").ToLower().Contains(pesquisa) ||
                _ilivroRepository.RemoverAcentos(l.Autor ?? "").ToLower().Contains(pesquisa) ||
                _ilivroRepository.RemoverAcentos(l.Genero ?? "").ToLower().Contains(pesquisa)
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

        const long tamanhoMaximoBytesImg = 128 * 1024;

        string caminhoImagem = null;

        //se a imagem for via IformFile
        if (foto != null && foto.Length> 0)
        {
            if(foto.Length > tamanhoMaximoBytesImg)
            {
                ViewBag.ErroImagem = "O tamanho da imagem deve ser menor que 120 KB.";
                return View(model);
            }

            caminhoImagem = await GeradorImagemAsync(foto);
        }

        //Se a imagem for via link api googleBooks

        if (string.IsNullOrEmpty(caminhoImagem) && !string.IsNullOrEmpty(model.Imagem))
        {
            caminhoImagem = model.Imagem;
        }
       

        var livro = new LivroModel
        {
            Name = _ilivroRepository.RemoverAcentos(model.Name ?? "").ToLower(),
            Autor = _ilivroRepository.RemoverAcentos(model.Autor ?? "").ToLower(),
            Genero = _ilivroRepository.RemoverAcentos(model.Genero ?? "").ToLower(),
            Idioma = _ilivroRepository.RemoverAcentos(model.Idioma ?? "").ToLower(),
            LivroFinalizado = model.LivroFinalizado,
            Imagem = caminhoImagem,
            ano = model.ano,
            Sinopese = model.Sinopese,
            Comentarios = model.Comentarios,
            Avaliacao = model.Avaliacao,
            NumeroPaginas = model.NumeroPaginas,
            Favorito = model.Favorito,
            TrechosFavoritos = model.TrechosFavoritos
        };
            await _ilivroRepository.AddLivroAsync(livro, usuarioId);
            return RedirectToAction("Index");
    }

    [HttpGet("Livro/BuscarPorNome")]
    public async Task<IActionResult> BuscarPorNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome)) return BadRequest("Nome inválido");

        var livroEncontrado = await _iGoogleBooksService.BuscarDadosDoLivroAsync(nome);

        if(livroEncontrado == null)
        {
            return NotFound("Livro não encontradoooo. Preencha os dados manualmente.");
        }

        return Ok(new
        {
            name = livroEncontrado.Name,
            autor = livroEncontrado.Autor,
            genero = livroEncontrado.Genero,
            sinopese = livroEncontrado.Sinopese,
            imagem = livroEncontrado.Imagem,
            numeroPaginas = livroEncontrado.NumeroPaginas,
            ano = livroEncontrado.ano
        });
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
        var usuarioId = HttpContext.Session.GetString("UsuarioId")!;
        if (usuarioId == null)
            return RedirectToAction("Login", "Usuario");

        var livroOriginal = await _ilivroRepository.GetByIdAsync(id, usuarioId);

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
        await _ilivroRepository.RemoveAsync(id, usuarioId);

        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Details(string id, int? page)
    {
        var usuarioId = HttpContext.Session.GetString("UsuarioId")!;
        if (usuarioId == null)
            return RedirectToAction("Login", "Usuario");


        var livro = await _ilivroRepository.GetByIdAsync(id, usuarioId);

        if (livro == null)
            return Unauthorized();

        int pageSize = 2;
        int pageNumber = page ?? 1;

        IPagedList<string> trechosPaginados = new List<string>().ToPagedList(pageNumber, pageSize);

        if(livro.TrechosFavoritos != null && livro.TrechosFavoritos.Any())
        {
            trechosPaginados = livro.TrechosFavoritos
                .OrderBy(t => t)
                .ToPagedList(pageNumber, pageSize);
        }

        ViewData["TrechoPage"] = trechosPaginados.PageNumber;
        ViewData["TrechoTotalPages"] = trechosPaginados.PageCount;
        ViewData["TrechosPaginados"] = trechosPaginados;

        return View(livro);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var usuarioId = HttpContext.Session.GetString("UsuarioId")!;

        if (usuarioId == null)
            return RedirectToAction("Login", "Usuario");


        var livro = await _ilivroRepository.GetByIdAsync(id, usuarioId);

        if (livro == null)
            return Unauthorized();

        return View(livro);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(string id, LivroModel livro, IFormFile? foto, List<string> trechosFavoritos)
    {
        var usuarioId = HttpContext.Session.GetString("UsuarioId")!;
        if (usuarioId == null)
            return RedirectToAction("Login", "Usuario");

        var livroOriginal = await _ilivroRepository.GetByIdAsync(id, usuarioId);

        if (livroOriginal == null)
        {
            TempData["Erro"] = "Livro não encontrado!";
            return RedirectToAction("Index");
        }

        const long tamanhoMaximoBytesImg = 128 * 1024;
        string caminhoImagem = null;

        // Se a imagem for via IFormFile
        if (foto != null && foto.Length > 0)
        {
            if (foto.Length > tamanhoMaximoBytesImg)
            {
                ViewBag.ErroImagem = "O tamanho da imagem deve ser menor que 120 KB.";
                return View(livroOriginal);
            }

            // Remove imagem antiga do disco (se existir)
            if (!string.IsNullOrEmpty(livroOriginal.Imagem) && !livroOriginal.Imagem.StartsWith("http"))
            {
                string caminhoAntigo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", livroOriginal.Imagem.TrimStart('/'));

                if (System.IO.File.Exists(caminhoAntigo))
                {
                    System.IO.File.Delete(caminhoAntigo);
                }
            }

            caminhoImagem = await GeradorImagemAsync(foto);
        }
        else if (!string.IsNullOrEmpty(livro.Imagem))
        {
            // Se imagem veio via link do formulário
            caminhoImagem = livro.Imagem;
        }
        else
        {
            // Nenhuma imagem nova, mantém a anterior
            caminhoImagem = livroOriginal.Imagem;
        }

  
        livro.Imagem = caminhoImagem;
        livro.Id = livroOriginal.Id;
        livro.UsuarioId = usuarioId;
        livro.TrechosFavoritos = trechosFavoritos?.Where(t => !string.IsNullOrWhiteSpace(t)).ToList() ?? new List<string>();

        await _ilivroRepository.UpdateAsync(id, livro, usuarioId);
        return RedirectToAction("Index");
    }

    [AllowAnonymous]
    [HttpGet("Livro/MeusFavoritos/{usuarioId}")]
    public async Task<IActionResult> MeusFavoritos(string usuarioId)
    {
        
        if (string.IsNullOrEmpty(usuarioId))
            return RedirectToAction("Login", "Usuario");

        var favoritos = await _ilivroRepository.ObterFavoritosDoUsuarioAsync(usuarioId);
        
        if (!favoritos.Any())
        {
            ViewBag.Mensagem = "Você ainda não marcou nenhum livro como favorito.";
        }

        return View("MeusFavoritos", favoritos);
    }

    [AllowAnonymous]
    [HttpGet("Livro/MeusFavoritosDetails/{usuarioId}/{id}")]
    public async Task<IActionResult>MeusFavoritosDetails(string id, string usuarioId, int? page)
    {
        if (usuarioId == null)
            return RedirectToAction("Login", "Usuario");

        var livro = await _ilivroRepository.GetByIdAsync(id, usuarioId);

        if (livro == null)
            return NotFound("Livro não encontrado ou não pertence ao usuário informado.");


        int pageSize = 2;
        int pageNumber = page ?? 1;

        IPagedList<string> trechosPaginados = new List<string>().ToPagedList(pageNumber, pageSize);

        if (livro.TrechosFavoritos != null && livro.TrechosFavoritos.Any())
        {
            trechosPaginados = livro.TrechosFavoritos
                .OrderBy(t => t)
                .ToPagedList(pageNumber, pageSize);
        }

        ViewData["TrechoPage"] = trechosPaginados.PageNumber;
        ViewData["TrechoTotalPages"] = trechosPaginados.PageCount;
        ViewData["TrechosPaginados"] = trechosPaginados;

        return View(livro);
    }
}
