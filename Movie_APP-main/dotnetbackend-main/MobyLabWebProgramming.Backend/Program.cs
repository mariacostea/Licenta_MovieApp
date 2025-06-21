using MobyLabWebProgramming.Core.Configuration;
using MobyLabWebProgramming.Infrastructure.Configurations;
using MobyLabWebProgramming.Infrastructure.Extensions;
using MobyLabWebProgramming.Infrastructure.Services.Implementations;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;
using MobyLabWebProgramming.Infrastructure.Workers;
using MobyLabWebProgramming.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<HostOptions>(opt =>
{
    opt.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
});

#region CORS
const string FrontendPolicy = "FrontendPolicy";

builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendPolicy, p =>
        p.WithOrigins("http://localhost:5173",
    "https://licenta-frontend-r2gn.onrender.com")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

#endregion

#region Servicii & infrastructură
builder.AddCorsConfiguration()
       .AddRepository()
       .AddAuthorizationWithSwagger("MobyLab Web App")
       .AddServices()
       .UseLogger()
       .AddWorkers();

builder.Services.AddControllers();
builder.Services.AddDbContext<WebAppDatabaseContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.AddApi();

builder.Services.Configure<TMDBConfiguration>(builder.Configuration.GetSection("TMDB"));
builder.Services.Configure<MailConfiguration>(builder.Configuration.GetSection("MailConfiguration"));

builder.Services.AddScoped<IMovieService,   MovieService>();
builder.Services.AddScoped<ICrewService,   CrewService>();
builder.Services.AddScoped<IReviewService,  ReviewService>();
builder.Services.AddScoped<ITmdbSeederService, TmdbSeederService>();
builder.Services.AddHostedService<TmdbImportWorker>();
builder.Services.AddScoped<IEventService,   EventService>();
builder.Services.AddScoped<IFriendshipService, FriendshipService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<IFeedService, FeedService>();
builder.Services.AddScoped<IRecommendationsService, RecommendationsService>();

#endregion

var app = builder.Build();

/*using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WebAppDatabaseContext>();
    var worker = new OmdbRatingWorker(db);
    await worker.RunAsync();
}



#region Seed TMDB
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<ITmdbSeederService>();
    await seeder.SeedGenresAndMoviesAsync();
}
#endregion
*/
#region Pipeline
app.UseCors(FrontendPolicy);

app.ConfigureApplication();

app.MapControllers();
#endregion

app.Run();