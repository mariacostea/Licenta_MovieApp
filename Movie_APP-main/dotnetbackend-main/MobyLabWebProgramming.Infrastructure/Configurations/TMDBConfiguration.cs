namespace MobyLabWebProgramming.Core.Configuration
{
    public class TMDBConfiguration
    {
        public string ApiKey { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = "https://api.themoviedb.org/3";
    }
}