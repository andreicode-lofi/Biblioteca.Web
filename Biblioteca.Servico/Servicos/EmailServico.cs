using System.Net;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Biblioteca.Servico.Servicos;

public class EmailServico
{
    private readonly string _smtpServidor = "smtp.gmail.com";
    private readonly int _porta = 587;
    private readonly string _usuario = "litmapbot@gmail.com";
    private readonly string _senha = "jnghmlgsxamolqih";

    public async Task EnviarAsync(string destino, string assunto, string linkRedefinicao)
    {
        using (var cliente = new SmtpClient(_smtpServidor, _porta))
        {
            cliente.Credentials = new NetworkCredential(_usuario, _senha);
            cliente.EnableSsl = true; // Para segurança

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_usuario),
                Subject = assunto,
                Body = $@"<html>
                            <body style='font-family: Arial, sans-serif; color: #333;'>
                                <h2>Redefinição de Senha</h2>
                                <p>Você solicitou a redefinição da sua senha.</p>
                                <p>Clique no botão abaixo para continuar com a redefinição:</p>
                                <p>
                                    <a href='{linkRedefinicao}' style='
                                        display: inline-block;
                                        padding: 10px 20px;
                                        background-color: #4CAF50;
                                        color: white;
                                        text-decoration: none;
                                        border-radius: 5px;
                                    '>Redefinir Senha</a>
                                </p>
                                <p>Se você não solicitou isso, pode ignorar este e-mail.</p>
                                <br>
                                <p>Atenciosamente,<br>Lit map Suporte</p>
                            </body>
                        </html>",
                IsBodyHtml = true,// se for preciso mandar link e html
            };

            mailMessage.To.Add(destino);

            await cliente.SendMailAsync(mailMessage);
        }
    }
}
//http://localhost:5011