namespace MobyLabWebProgramming.Infrastructure.Services.Interfaces;

public interface ITmdbSeederService
{
    Task SeedGenresAndMoviesAsync();
}
