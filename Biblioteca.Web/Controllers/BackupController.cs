using Biblioteca.Servico.Servicos;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Web.Controllers;

[Route("Backup")]
public class BackupController : Controller
{
    private readonly BackupLivro _backupService;

    public BackupController(BackupLivro backupService)
    {
        _backupService = backupService;
    }

    [HttpPost("Restaurar")]
    public IActionResult Restaurar()
    {
        var diretorioBackup = "Backups";
        var backups = Directory.GetFiles(diretorioBackup, "backup_livros_*.json")
            .OrderByDescending(arquivo => System.IO.File.GetCreationTime(arquivo))
            .ToList();

        if (!backups.Any())
        {
            TempData["MensagemErro"] = "Nenhum arquivo de backup encontrado.";
            return RedirectToAction("Index", "Livro");
        }

        var caminhoMaisRecente = backups.First();
        var nomeMaisRecente = Path.GetFileName(caminhoMaisRecente);

        _backupService.RestaurarBackup(nomeMaisRecente);

        TempData["MensagemSucesso"] = "Backup restaurado com sucesso!";
        return RedirectToAction("Index", "Livro");
    }
}