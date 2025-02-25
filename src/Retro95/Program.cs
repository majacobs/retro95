using Microsoft.EntityFrameworkCore;
using Retro95.Models.Db;

namespace Retro95;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<RetroContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("Retro")));

        // Add services to the container.
        builder.Services.AddRazorPages();

        var app = builder.Build();
        
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
        
            var context = services.GetRequiredService<RetroContext>();
            await context.Database.EnsureCreatedAsync();
        }

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
        }

        app.UseRouting();

        app.MapStaticAssets();
        app.MapRazorPages()
            .WithStaticAssets();
        
        await app.RunAsync();
    }
}