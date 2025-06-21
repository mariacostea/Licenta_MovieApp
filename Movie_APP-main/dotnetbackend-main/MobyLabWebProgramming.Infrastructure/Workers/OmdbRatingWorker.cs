using System.Net.Http;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MobyLabWebProgramming.Infrastructure.Database;
using MobyLabWebProgramming.Core.Entities;

public class OmdbRatingWorker
{
    private readonly WebAppDatabaseContext _db;
    private readonly HttpClient _http;
    private readonly string _apiKey = "33f9d89f";

    public OmdbRatingWorker(WebAppDatabaseContext db)
    {
        _db = db;
        _http = new HttpClient();
    }

    public async Task RunAsync()
    {
        var movies = await _db.Movies
            .Where(m => m.TmdbId == 0 && !string.IsNullOrWhiteSpace(m.Title))
            .OrderBy(m => m.Id)
            .Skip(3000)
            .Take(0)
            .ToListAsync();

        Console.WriteLine($"[OMDb] Filme de procesat: {movies.Count}");

        if (movies.Count == 0)
        {
            Console.WriteLine("[OMDb] Nu există filme de actualizat.");
            return;
        }

        int updated = 0;

        foreach (var movie in movies)
        {
            var url = $"https://www.omdbapi.com/?t={Uri.EscapeDataString(movie.Title)}&y={movie.Year}&apikey={_apiKey}";

            try
            {
                var response = await _http.GetAsync(url);
                if (!response.IsSuccessStatusCode) continue;

                var content = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(content);
                var root = doc.RootElement;

                if (root.TryGetProperty("imdbRating", out var ratingProp)
                    && double.TryParse(ratingProp.GetString(), out double rating))
                {
                    movie.TmdbId = (int)(rating * 10);
                    updated++;
                    Console.WriteLine($"[✔] {movie.Title} ({movie.Year}) => {rating}");
                }
                else
                {
                    Console.WriteLine($"[!] Fără rating: {movie.Title} ({movie.Year})");
                }

                await Task.Delay(1000);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[✘] {movie.Title}: {ex.Message}");
            }
        }

        await _db.SaveChangesAsync();
        Console.WriteLine($"[OMDb] Finalizat. Filme actualizate: {updated}");
    }

}
