using Biblioteca.Servico.model;
using Biblioteca.Servico.Servicos;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Web.Controllers;

public class UsuarioLoginController : Controller
{
    private readonly GerenciadorDeUsuarios _gerenciadorDeUsuarios;

    public UsuarioLoginController(GerenciadorDeUsuarios gerenciadorDeUsuarios)
    {
        _gerenciadorDeUsuarios = gerenciadorDeUsuarios;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email, string senha)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
        {
            TempData["Erro"] = "E-mail e senha são obrigatorios!";
            return RedirectToAction("Index");
        }

        UsuarioModel? usuario = await _gerenciadorDeUsuarios.loginAsync(email, senha);

        if (usuario != null)
        {
            return RedirectToAction("Index", "Livro");
        }
        else
        {
            TempData["Erro"] = "E-mail ou senha inválidos!";
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public async Task<IActionResult> RegistrarUsuario(string nome, string email, string senha)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha) || string.IsNullOrEmpty(nome))
        {
            TempData["Erro"] = "E-mail senha e nome são obrigatorios!";
            return RedirectToAction("Index");
        }

        bool novoUsuario = await _gerenciadorDeUsuarios.registrarUsuarioAsync(nome, email, senha);

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
}

