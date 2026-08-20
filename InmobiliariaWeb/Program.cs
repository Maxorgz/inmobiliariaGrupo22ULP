using Microsoft.EntityFrameworkCore;
using InmobiliariaWeb.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuración de la base de datos (ajústala según lo que usen tus compañeros)
var connectionString = builder.Configuration.GetConnectionString("MySqlConn");
// builder.Services.AddDbContext<DataContext>(options => ...); // Comentado temporalmente si usan otro conector

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();