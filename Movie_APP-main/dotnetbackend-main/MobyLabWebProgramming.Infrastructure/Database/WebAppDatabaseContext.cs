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
    
    public DbSet<UserMovie> UserMovies { get; set; } = default!;
    public DbSet<User> Users { get; set; } = default!;
    public DbSet<Movie> Movies { get; set; } = default!;
    public DbSet<Friendship> Friendships { get; set; } = default!;
    public DbSet<Review> Reviews { get; set; } = default!;
    public DbSet<Event> Events { get; set; } = default!;
    public DbSet<Notification> Notifications { get; set; } = default!;
    public DbSet<Genre> Genres { get; set; } = default!;
    public DbSet<MovieGenre> GenreMovies { get; set; } = default!;
    public DbSet<Crew> Crews { get; set; } = default!;
    public DbSet<MovieCrew> MovieCrews { get; set; } = default!;
    public DbSet<Feedback> Feedback { get; set; } = default!;
    public DbSet<UserEvent> UserEvents { get; set; } = default!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasPostgresExtension("unaccent")
            .ApplyAllConfigurationsFromCurrentAssembly();
    }
}

