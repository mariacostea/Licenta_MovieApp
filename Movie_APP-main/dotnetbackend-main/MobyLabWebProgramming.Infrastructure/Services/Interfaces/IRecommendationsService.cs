namespace MobyLabWebProgramming.Infrastructure.Services.Interfaces
{
public interface IRecommendationsService
{
    Task<List<MovieDetailsDTO>> GetRecommendedByGenre(Guid userId);
    Task<List<MovieDetailsDTO>> GetRecommendedByActors(Guid userId);
    Task<List<MovieDetailsDTO>> GetRecommendedByDescription(Guid userId);
    Task<List<MovieDetailsDTO>> GetCombinedRecommendations(Guid userId);
}
}