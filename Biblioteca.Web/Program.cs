using Biblioteca.Servico.Servicos;
using Biblioteca.Web.Sessao;

var builder = WebApplication.CreateBuilder(args);

// Configurando os serviços sessço----------------------------------
builder.Services.AddDistributedMemoryCache(); // Necessario para sessçes em memoria
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tempo de expiração da sessço
    options.Cookie.HttpOnly = true; // Torna o cookie acessivel apenas pelo servidor
});

builder.Services.AddHttpContextAccessor(); // Para usar IHttpContextAccessor
//-----------------------------------------------------------------------------

builder.Services.AddScoped<GerenciadorDeSessao>();
builder.Services.AddScoped<GerenciadorDelivros>();
builder.Services.AddScoped<GerenciadorDeUsuarios>();
// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();


app.UseSession();// Habilitando o uso de sess�es
app.UseAuthorization();

app.MapStaticAssets();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=UsuarioLogin}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
