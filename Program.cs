using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SalesWebMvc.Data;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// 1. Pegamos a string de conexão primeiro para organizar
var connectionString = builder.Configuration.GetConnectionString("SalesWebMvcContext");

// 2. Configuramos o DbContext
builder.Services.AddDbContext<SalesWebMvcContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString), // <-- Aqui entra o ServerVersion
        builder => builder.MigrationsAssembly("SalesWebMvc")
    )
);

builder.Services.AddScoped<SeedingService>();

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
else
{
    // Aquele método Configure não existe mais como uma função separada.
    // Para rodar o Seed no Program.cs moderno, fazemos assim:
    using (var scope = app.Services.CreateScope())
    {
        var seedingService = scope.ServiceProvider.GetRequiredService<SeedingService>();
        seedingService.Seed();
    }
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
