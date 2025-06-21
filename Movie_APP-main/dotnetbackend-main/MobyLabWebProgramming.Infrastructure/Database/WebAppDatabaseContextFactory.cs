using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using MobyLabWebProgramming.Infrastructure.Database;
using System.IO;

namespace MobyLabWebProgramming.Infrastructure
{
    public class WebAppDatabaseContextFactory : IDesignTimeDbContextFactory<WebAppDatabaseContext>
    {
        public WebAppDatabaseContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) 
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<WebAppDatabaseContext>();
            optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));

            return new WebAppDatabaseContext(optionsBuilder.Options);
        }
    }
}