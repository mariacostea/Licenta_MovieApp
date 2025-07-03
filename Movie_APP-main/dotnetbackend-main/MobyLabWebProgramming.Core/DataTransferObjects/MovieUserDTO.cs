namespace MobyLabWebProgramming.Core.DataTransferObjects;

public class MovieUserDto {
    public string Id { get; set; }
    public string Title { get; set; }
    public int Year { get; set; }
    public List<string> Genres { get; set; }
    public string PosterUrl { get; set; }
    public bool IsRecommended { get; set; }
    public bool IsWatched { get; set; }
    public float Rating { get; set; }
}
