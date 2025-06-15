using MobyLabWebProgramming.Infrastructure.Services.Interfaces;
using MobyLabWebProgramming.Core.Specifications;
using MobyLabWebProgramming.Infrastructure.Repositories.Interfaces;
using MobyLabWebProgramming.Infrastructure.Database;

namespace MobyLabWebProgramming.Infrastructure.Services.Implementations
{
    public class RecommendationsService : IRecommendationsService
    {
        private readonly IRepository<WebAppDatabaseContext> _movieRepo;

        public RecommendationsService(IRepository<WebAppDatabaseContext> movieRepo)
        {
            _movieRepo = movieRepo;
        }

        public async Task<List<MovieDetailsDTO>> GetRecommendedByGenre(Guid userId)
        {
            var watchedMovies = await _movieRepo.ListAsync(new MoviesWatchedByUserSpec(userId));
            
            var ratedMovies = watchedMovies
                .Where(m => m.Reviews?.Any(r => r.UserId == userId) == true)
                .ToList();
            
            var preferredGenres = ratedMovies
                .SelectMany(m => m.MovieGenres.Select(g => new
                {
                    Genre = g.Genre?.Name,
                    Rating = m.Reviews!.First(r => r.UserId == userId).Rating
                }))
                .Where(x => x.Genre != null)
                .GroupBy(x => x.Genre!)
                .Select(g => new { Genre = g.Key, Avg = g.Average(x => x.Rating) })
                .OrderByDescending(g => g.Avg)
                .Take(3)
                .Select(g => g.Genre)
                .ToList();

            if (!preferredGenres.Any())
                return new List<MovieDetailsDTO>();
            
            var allMovies = await _movieRepo.ListAsync(new MoviesNotWatchedByUserSpec(userId));

            var matchedMovies = allMovies
                .Where(m =>
                    m.MovieGenres != null &&
                    m.MovieGenres.Select(g => g.Genre?.Name)
                        .Intersect(preferredGenres)
                        .Count() >= 2)
                .OrderByDescending(m =>
                    m.Reviews?.Any() == true
                        ? m.Reviews.Average(r => r.Rating)
                        : (m.TmdbId != 0 ? m.TmdbId / 10.0 : 0)
                )
                .Take(10)
                .ToList();

            return matchedMovies.Select(MapToDetailsDTO).ToList();
        }

        public async Task<List<MovieDetailsDTO>> GetRecommendedByActors(Guid userId)
        {
            var watched = await _movieRepo.ListAsync(new MoviesWatchedByUserSpec(userId));
            
            var actorScores = new Dictionary<Guid, int>();

            foreach (var movie in watched)
            {
                var userRating = movie.Reviews?.FirstOrDefault(r => r.UserId == userId)?.Rating ?? 0;

                int score = userRating switch
                {
                    >= 9 => 5,
                    >= 7 => 4,
                    >= 6 => 3,
                    _ => 0
                };

                foreach (var mc in movie.MovieCrews ?? new List<MovieCrew>())
                {
                    if (score == 0 || mc.Crew == null) continue;

                    if (!actorScores.ContainsKey(mc.CrewId))
                        actorScores[mc.CrewId] = 0;

                    actorScores[mc.CrewId] += score;
                }
            }

            if (!actorScores.Any())
                return new List<MovieDetailsDTO>();
            
            var unwatched = await _movieRepo.ListAsync(new MoviesNotWatchedByUserSpec(userId));
            
            var recommended = unwatched
                .Select(m => new
                {
                    Movie = m,
                    Score = m.Reviews?.Any() == true
                        ? m.Reviews.Average(r => r.Rating)
                        : 0,
                    ActorMatchScore = m.MovieCrews?
                        .Where(mc => actorScores.ContainsKey(mc.CrewId))
                        .Sum(mc => actorScores[mc.CrewId]) ?? 0
                })
                .OrderByDescending(x => x.ActorMatchScore) // prioritar: actori apreciați
                .ThenByDescending(x => x.Score) // secundar: review (dacă există)
                .Take(10)
                .Select(x => x.Movie)
                .ToList();

            return recommended.Select(MapToDetailsDTO).ToList();
        }



public async Task<List<MovieDetailsDTO>> GetRecommendedByDescription(Guid userId)
{
    var watched = await _movieRepo.ListAsync(new MoviesWatchedByUserSpec(userId));
    var rated = watched
        .Where(m => m.Reviews?.Any(r => r.UserId == userId) == true)
        .ToList();
    
    var stopWords = new HashSet<string> {
        "the", "and", "is", "in", "at", "of", "a", "an", "to", "with", "by", "on", "for", "as", "this", "that", "these", "those",
        "he", "she", "it", "they", "we", "you", "i", "his", "her", "its", "their", "our", "your", "mine", "me", "my", "us"
    };

    var wordFreq = rated
        .Where(m => !string.IsNullOrWhiteSpace(m.Description))
        .SelectMany(m => m.Description!
            .ToLower()
            .Split(new[] { ' ', '.', ',', '!', '?', ':', ';', '-', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
            .Where(w => w.Length > 2 && !stopWords.Contains(w))
        )
        .GroupBy(w => w)
        .Select(g => new { Word = g.Key, Count = g.Count() })
        .OrderByDescending(g => g.Count)
        .Take(10)
        .Select(g => g.Word)
        .ToList();

    if (!wordFreq.Any())
        return new List<MovieDetailsDTO>();
    
    var allUnwatched = await _movieRepo.ListAsync(new MoviesNotWatchedByUserSpec(userId));
    
    var matched = allUnwatched
        .Where(m => !string.IsNullOrWhiteSpace(m.Description))
        .Select(m => new
        {
            Movie = m,
            MatchCount = wordFreq.Count(w => m.Description!.ToLower().Contains(w))
        })
        .Where(x => x.MatchCount > 0)
        .OrderByDescending(x => x.MatchCount)
        .ThenByDescending(x =>
            x.Movie.Reviews?.Any() == true ? x.Movie.Reviews.Average(r => r.Rating) :
            (x.Movie.TmdbId != 0 ? x.Movie.TmdbId / 10.0 : 0)
        )
        .Take(10)
        .Select(x => x.Movie)
        .ToList();
    
    if (matched.Count < 10)
    {
        var backup = allUnwatched
            .Except(matched)
            .Where(m => !string.IsNullOrWhiteSpace(m.Description) &&
                        wordFreq.Any(w => m.Description!.ToLower().Contains(w)))
            .OrderByDescending(m =>
                m.Reviews?.Any() == true ? m.Reviews.Average(r => r.Rating) :
                (m.TmdbId != 0 ? m.TmdbId / 10.0 : 0)
            )
            .Take(10 - matched.Count)
            .ToList();

        matched.AddRange(backup);
    }

    return matched.Select(MapToDetailsDTO).ToList();
}


      public async Task<List<MovieDetailsDTO>> GetCombinedRecommendations(Guid userId)
{
    var watchedMovies = await _movieRepo.ListAsync(new MoviesWatchedByUserSpec(userId));
    var allUnwatched = await _movieRepo.ListAsync(new MoviesNotWatchedByUserSpec(userId));
    
    var topGenres = watchedMovies
        .SelectMany(m => m.MovieGenres.Select(g => new
        {
            Genre = g.Genre?.Name,
            Rating = m.Reviews?.FirstOrDefault(r => r.UserId == userId)?.Rating ?? 0
        }))
        .Where(x => x.Genre != null)
        .GroupBy(x => x.Genre!)
        .OrderByDescending(g => g.Average(x => x.Rating))
        .Take(3)
        .Select(g => g.Key)
        .ToHashSet();
    
    var actorScores = watchedMovies
        .SelectMany(m => m.MovieCrews.Select(mc => new
        {
            ActorId = mc.CrewId,
            Rating = m.Reviews?.FirstOrDefault(r => r.UserId == userId)?.Rating ?? 0
        }))
        .GroupBy(x => x.ActorId)
        .ToDictionary(
            g => g.Key,
            g => g.Sum(x => x.Rating switch
            {
                >= 9 => 5,
                >= 7 => 4,
                >= 5 => 3,
                _ => 0
            }));
    
    var stopWords = new HashSet<string> { "the", "and", "a", "of", "in", "on", "with", "to", "an", "at", "by", "from", "as", "for", "it", "is", "are" };
    var keywordCounts = watchedMovies
        .Where(m => m.Reviews?.Any(r => r.UserId == userId && r.Rating >= 8) == true)
        .SelectMany(m => (m.Description ?? "")
            .ToLower()
            .Split(new[] { ' ', ',', '.', '-', ':', ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
            .Where(word => !stopWords.Contains(word)))
        .GroupBy(word => word)
        .OrderByDescending(g => g.Count())
        .Take(10)
        .Select(g => g.Key)
        .ToList();
    
    var ranked = allUnwatched
        .Select(m =>
        {
            int score = 0;
            
            var movieGenres = m.MovieGenres.Select(g => g.Genre?.Name).Where(n => n != null).ToList();
            if (movieGenres.Intersect(topGenres).Count() >= 2) score += 5;
            
            score += m.MovieCrews
                .Where(mc => actorScores.ContainsKey(mc.CrewId))
                .Sum(mc => actorScores[mc.CrewId]);
            
            var descWords = (m.Description ?? "").ToLower().Split(' ');
            score += keywordCounts.Intersect(descWords).Count() * 2;
            
            double fallbackRating = m.Reviews?.Any() == true ? m.Reviews.Average(r => r.Rating) : (m.TmdbId > 0 ? m.TmdbId / 10.0 : 0);

            return new { Movie = m, Score = score, Rating = fallbackRating };
        })
        .OrderByDescending(x => x.Score)
        .ThenByDescending(x => x.Rating)
        .Take(10)
        .Select(x => MapToDetailsDTO(x.Movie))
        .ToList();

    return ranked;
}


        private static MovieDetailsDTO MapToDetailsDTO(Movie m)
        {
            return new MovieDetailsDTO
            {
                Id = m.Id,
                Title = m.Title,
                Year = m.Year,
                Description = m.Description,
                AverageRating = m.Reviews?.Any() == true ? m.Reviews.Average(r => r.Rating) : 0,
                PosterUrl = m.PosterUrl,
                Genres = m.MovieGenres?.Select(g => g.Genre?.Name ?? string.Empty).ToList() ?? new List<string>()
            };
        }
    }
}
