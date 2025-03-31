using Biblioteca.Servico.model;
using Newtonsoft.Json;

namespace Biblioteca.Web.Sessao
{
    public class GerenciadorDeSessao
    {
        private readonly IHttpContextAccessor _httpContext;
        public GerenciadorDeSessao(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }

        public void CriarSessaoDoUsuario(UsuarioModel usuario)
        {
            string valor = JsonConvert.SerializeObject(usuario);
            _httpContext.HttpContext?.Session.SetString("sessaoUsuarioLogado", valor); // Armazenando na sessão como string
        }

        public async Task<UsuarioModel> BuscarSessaoUsuario()
        {
            string? sessaoUsuario = _httpContext.HttpContext?.Session.GetString("sessaoUsuarioLogado");

            return string.IsNullOrEmpty(sessaoUsuario)
                ? null
                : JsonConvert.DeserializeObject<UsuarioModel>(sessaoUsuario);
        }

        public void RemoveSessaoUsuario()
        {
            _httpContext.HttpContext?.Session.Remove("sessaoUsuarioLogado");
        }
    }
}
