using Biblioteca.Servico.model;
using Biblioteca.Servico.Servicos;
using Biblioteca.Web.Sessao;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Web.Controllers;

public class UsuarioLoginController : Controller
{
    private readonly GerenciadorDeUsuarios _gerenciadorDeUsuarios;
    private readonly GerenciadorDeSessao _sessao;

    private readonly EmailServico _servicoEmail;

    public UsuarioLoginController(GerenciadorDeUsuarios gerenciadorDeUsuarios, GerenciadorDeSessao sessao, EmailServico servicoEmail)
    {
        _gerenciadorDeUsuarios = gerenciadorDeUsuarios;
        _sessao = sessao;
        _servicoEmail = servicoEmail;
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

    [HttpGet]
    public IActionResult SolicitarRedefiniSenha()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SolicitarRedeFiniSenha(string email)
    {

        var token = await _gerenciadorDeUsuarios.GerarTokenRedefinicaoAsync(email);

        if (token != null)
        {
            var link = Url.Action("RedefinirSenha", "UsuarioLogin", new { token }, Request.Scheme);

            await _servicoEmail.EnviarAsync(email, "Redefinir senha", link);

            TempData["MensagemSucesso"] = "Verifique seu e-mail para redefinir a senha.";
            return RedirectToAction("RedefinirSenha", new { token });

        }
        else
        {
            TempData["MensagemErro"] = "E-mail não encontrado.";
            return RedirectToAction("SolicitarRedefiniSenha");
        }
    }

    [HttpGet]
    public async Task<IActionResult> RedefinirSenha(string token)
    {

        var usuario = await _gerenciadorDeUsuarios.BuscarPorTokenAsync(token);

        if (usuario == null)
        {
            TempData["MensagemErro"] = "Token inválido ou expirado";
            return RedirectToAction("SolicitarRedefiniSenha");
        }

        ViewBag.Token = token;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> RedefinirSenha(string token, string novaSenha, string confirmaSenha)
    {
        if (novaSenha != confirmaSenha)
        {
            TempData["MensagemErro"] = "As senhas não coincidem.";
            ViewBag.Token = token;
            return View();
        }

        var usuario = await _gerenciadorDeUsuarios.BuscarPorTokenAsync(token);

        if (usuario == null)
        {
            TempData["MensagemErro"] = "Token inválido ou expirado.";
            return RedirectToAction("SolicitarRedefinicao");
        }

        usuario.SenhaHas = await _gerenciadorDeUsuarios.CriptografarSenha(novaSenha);
        usuario.TokenRedefinicao = null;
        usuario.TokenExperiracao = null;

        await _gerenciadorDeUsuarios.AtualizarUsuarioSenhaAsync(usuario);

        TempData["MensagemSucesso"] = "Senha redefinida com sucesso!";
        return RedirectToAction("Index");

    }

    public IActionResult Sair()
    {
        _sessao.RemoveSessaoUsuario();

        return RedirectToAction("Index", "UsuarioLogin");
    }

    /*[HttpPost]
    public async Task<IActionResult> RedefinirSenha(RedefinirSenhaModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["MensagemErro"] = "Verifique os campos e tente novamente.";
            return View(model);
        }

        var usuario = await _gerenciadorDeUsuarios.BuscarPorEmailAsync(model.Email);

        usuario.SenhaHas = await _gerenciadorDeUsuarios.CriptografarSenha(model.NovaSenha);

        await _gerenciadorDeUsuarios.AtualizarUsuarioSenhaAsync(usuario);

        TempData["MensagemSucesso"] = "Senha redefinida com sucesso! Você já pode fazer login.";
        return RedirectToAction("Index", "Livro");
    }*/

}

