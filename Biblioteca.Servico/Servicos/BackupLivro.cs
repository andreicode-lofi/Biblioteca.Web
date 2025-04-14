namespace Biblioteca.Servico.Servicos;
public class BackupLivro
{
    private readonly string _caminhoArquivoOriginal = "Biblioteca.json";
    private readonly string _diretorioBackup = "Backups";

    public void CriarBackup()
    {
        if (!File.Exists(_caminhoArquivoOriginal))
            return;

        if (!Directory.Exists(_diretorioBackup))
            Directory.CreateDirectory(_diretorioBackup);

        string dataHora = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string nomeBackup = $"backup_livros_{dataHora}.json";
        string caminhoBackup = Path.Combine(_diretorioBackup, nomeBackup);

        File.Copy(_caminhoArquivoOriginal, caminhoBackup);

        //Remover o mais antigos, se houver mais de 5
        var arquivosBackup = Directory.GetFiles(_diretorioBackup, "backup_livros_*.json")
                                        .OrderByDescending(arquivo => File.GetCreationTime(arquivo))
                                        .ToList();

        if (arquivosBackup.Count > 5)
        {
            var arquivosParaApagar = arquivosBackup.Skip(5);
            foreach (var arquivo in arquivosParaApagar)
            {
                File.Delete(arquivo);
            }
        }
    }
}