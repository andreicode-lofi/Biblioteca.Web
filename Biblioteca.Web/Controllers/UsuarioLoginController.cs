using Biblioteca.Servico.Servicos;
using Biblioteca.Web.Models;
using Biblioteca.Web.Repository.Interface;
using Biblioteca.Web.Sessao;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Web.Controllers;

public class UsuarioLoginController : Controller
{
    private readonly GerenciadorDeSessao _sessao;
    private readonly EmailServico _servicoEmail;
    private readonly IUsuarioRepository _iusuarioRepository;

    public UsuarioLoginController(
    GerenciadorDeSessao sessao, EmailServico servicoEmail,
    IUsuarioRepository usuarioRepository
    )
    {
        _sessao = sessao;
        _servicoEmail = servicoEmail;
        _iusuarioRepository = usuarioRepository;
    }

    [HttpGet]
    public IActionResult Index()
    {

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

        UsuarioModel? usuario = await _iusuarioRepository.LoginAsync(email, senhaHas);

        if (usuario != null)
        {
            _sessao.CriarSessaoDoUsuario(usuario);
            HttpContext.Session.SetString("UsuarioId", usuario.Id.ToString());
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

        bool novoUsuario = await _iusuarioRepository.RegistrarUsuarioAsync(nome, email, senhaHas);

        if (novoUsuario)
        {
            return RedirectToAction("Index");
        }
        else
        {
            TempData["Erro"] = "Erro ao registrar sua conta!";
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public async Task<IActionResult> ExcluirUsuario()
    {
        var usuarioId = HttpContext.Session.GetString("UsuarioId");

        if (usuarioId == null)
            return RedirectToAction("Login", "Usuario");

        if (string.IsNullOrEmpty(usuarioId))
        {
            TempData["Erro"] = "Usuário não está logado.";
            return RedirectToAction("Index", "Login");
        }

        var sucesso = await _iusuarioRepository.ExcluirUsuarioAsync(usuarioId);

        if (sucesso)
        {
            // Limpa a sessão após exclusão
            HttpContext.Session.Clear();
            TempData["Sucesso"] = "Conta excluída com sucesso.";
            return RedirectToAction("Index", "UsuarioLogin");
        }
        else
        {
            TempData["Erro"] = "Erro ao excluir sua conta.";
            return RedirectToAction("Index", "Livro");
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
        var token = await _iusuarioRepository.GerarTokenRedefinicaoAsync(email);

        if (token != null)
        {
            var link = Url.Action("RedefinirSenha", "UsuarioLogin", new { token }, Request.Scheme);

            await _servicoEmail.EnviarAsync(email, "Redefinir senha", link);

            TempData["MensagemSucesso"] = "Verifique seu e-mail para redefinir a senha.";
            return RedirectToAction("SolicitarRedefiniSenha");

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
        var usuario = await _iusuarioRepository.BuscarPorTokenAsync(token);

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

        var usuario = await _iusuarioRepository.BuscarPorTokenAsync(token);

        if (usuario == null)
        {
            TempData["MensagemErro"] = "Token inválido ou expirado.";
            return RedirectToAction("SolicitarRedefinicao");
        }

        usuario.SenhaHas = await _iusuarioRepository.CriptografarSenha(novaSenha);
        usuario.TokenRedefinicao = null;
        usuario.TokenExperiracao = null;

        await _iusuarioRepository.AtualizarUsuarioSenhaAsync(usuario);

        TempData["MensagemSucesso"] = "Senha redefinida com sucesso!";
        return RedirectToAction("Index");

    }

    public IActionResult Sair()
    {
        _sessao.RemoveSessaoUsuario();

        return RedirectToAction("Index", "UsuarioLogin");
    }
}

