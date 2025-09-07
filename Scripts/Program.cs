using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using irevlogix_backend.Data;
using irevlogix_backend.Scripts;

namespace irevlogix_backend.Scripts
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                Console.WriteLine("Error: Connection string 'DefaultConnection' not found in configuration.");
                Console.WriteLine("Please ensure appsettings.json contains a valid connection string.");
                return;
            }

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));

            builder.Services.AddScoped<UpdateKnowledgeBaseArticles>();

            var app = builder.Build();

            try
            {
                using var scope = app.Services.CreateScope();
                var updateService = scope.ServiceProvider.GetRequiredService<UpdateKnowledgeBaseArticles>();
                
                Console.WriteLine("Starting Knowledge Base Articles update...");
                await updateService.UpdateArticlesAsync();
                Console.WriteLine("Update completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during update: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return;
            }
        }
    }
}
