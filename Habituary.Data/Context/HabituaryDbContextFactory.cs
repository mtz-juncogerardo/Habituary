using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Habituary.Data.Context
{
    public class HabituaryDbContextFactory : IDesignTimeDbContextFactory<HabituaryDbContext>
    {
        public HabituaryDbContext CreateDbContext(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Habituary.API");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no se encontró o está vacía.");

            var optionsBuilder = new DbContextOptionsBuilder<HabituaryDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new HabituaryDbContext(optionsBuilder.Options);
        }
    }
}