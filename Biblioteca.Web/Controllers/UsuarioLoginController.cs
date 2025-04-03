using Biblioteca.Servico.model;
using Biblioteca.Servico.Servicos;
using Biblioteca.Web.Sessao;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Web.Controllers;

public class UsuarioLoginController : Controller
{
    private readonly GerenciadorDeUsuarios _gerenciadorDeUsuarios;
    private readonly GerenciadorDeSessao _sessao;

    public UsuarioLoginController(GerenciadorDeUsuarios gerenciadorDeUsuarios, GerenciadorDeSessao sessao)
    {
        _gerenciadorDeUsuarios = gerenciadorDeUsuarios;
        _sessao = sessao;
    }

    [HttpGet]
    public IActionResult Index()
    {
        //Se usuario estiver loogado, redirecionar para home
        if (_sessao.BuscarSessaoUsuario() != null)
        {
            return RedirectToAction("Index", "Livro");
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email, string senhaHas)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senhaHas))
        {
            TempData["Erro"] = "E-mail e senha são obrigatorios!";
            return RedirectToAction("Index");
        }

        UsuarioModel? usuario = await _gerenciadorDeUsuarios.loginAsync(email, senhaHas);

        if (usuario != null)
        {
            _sessao.CriarSessaoDoUsuario(usuario);
            return RedirectToAction("Index", "Livro");
        }
        else
        {
            TempData["Erro"] = "E-mail ou senha inválidos!";
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public async Task<IActionResult> RegistrarUsuario(string nome, string email, string senhaHas)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senhaHas) || string.IsNullOrEmpty(nome))
        {
            TempData["Erro"] = "E-mail senha e nome são obrigatorios!";
            return RedirectToAction("Index");
        }

        bool novoUsuario = await _gerenciadorDeUsuarios.registrarUsuarioAsync(nome, email, senhaHas);

        if (novoUsuario)
        {
            return RedirectToAction("Index", "Livro");
        }
        else
        {
            TempData["Erro"] = "Erro ao registrar sua conta!";
            return RedirectToAction("Index");
        }
    }

    public IActionResult Sair()
    {


        _sessao.RemoveSessaoUsuario();

        return RedirectToAction("Index", "UsuarioLogin");
    }
}

