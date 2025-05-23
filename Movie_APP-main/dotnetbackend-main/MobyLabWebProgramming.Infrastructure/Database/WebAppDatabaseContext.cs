using Ardalis.EFCore.Extensions;
using Microsoft.EntityFrameworkCore;

namespace MobyLabWebProgramming.Infrastructure.Database;

/// <summary>
/// This is the database context used to connect with the database and links the ORM, Entity Framework, with it.
/// </summary>
    public sealed class WebAppDatabaseContext : DbContext
    {
        public WebAppDatabaseContext(DbContextOptions<WebAppDatabaseContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasPostgresExtension("unaccent")
                .ApplyAllConfigurationsFromCurrentAssembly();
        }
    }
