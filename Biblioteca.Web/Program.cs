using Biblioteca.Servico.Servicos;
using Biblioteca.Web.Context;
using Biblioteca.Web.Repository;
using Biblioteca.Web.Repository.Interface;
using Biblioteca.Web.Sessao;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//builder.WebHost.UseWebRoot("wwwroot");

// Configurando do banco de dados==================================================
string? pgSqlConnection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(pgSqlConnection)
);
//=================================================================================



// Configurando os serviços sessço==================================================
builder.Services.AddDistributedMemoryCache(); // Necessario para sessçes em memoria
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tempo de expiração da sessço
    options.Cookie.HttpOnly = true; // Torna o cookie acessivel apenas pelo servidor
});
builder.Services.AddHttpContextAccessor(); // Para usar IHttpContextAccessor
//=================================================================================


//Servicos e repositorios =========================================================
builder.Services.AddScoped<GerenciadorDeSessao>();
builder.Services.AddScoped<EmailServico>();
builder.Services.AddScoped<ILivroRepository, LivroRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IGoogleBooksService, GoogleBooksService>();
//=================================================================================
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

app.UseStaticFiles();         


app.UseRouting();

app.UseSession();// Habilitando o uso de sess�es

app.UseAuthorization();

//app.MapStaticAssets();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=UsuarioLogin}/{action=Index}/{id?}")
    .WithStaticAssets();


//======verificando se tem usuario inativos==============================================================================

using (var scope = app.Services.CreateScope())
{
    var userRepository = scope.ServiceProvider.GetRequiredService<IUsuarioRepository>();
    await userRepository.ExcluirUsuariosInativosAsync();
}


app.Run();
