using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MobyLabWebProgramming.Infrastructure.Database;

public sealed class WebAppDatabaseContextFactory
    : IDesignTimeDbContextFactory<WebAppDatabaseContext>
{
    public WebAppDatabaseContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<WebAppDatabaseContext>();
        
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=mobylab-app;Username=mobylab-app;Password=mobylab-app");

        return new WebAppDatabaseContext(optionsBuilder.Options);
    }
}
