using MobyLabWebProgramming.Core.Configuration;
using MobyLabWebProgramming.Infrastructure.Configurations;
using MobyLabWebProgramming.Infrastructure.Extensions;
using MobyLabWebProgramming.Infrastructure.Services.Implementations;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;
using MobyLabWebProgramming.Infrastructure.Workers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<HostOptions>(opt =>
{
    // Dacă un BackgroundService aruncă => continui să rulezi doar cu log WARN
    opt.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
});

#region ─── CORS (Vite localhost:5173) ─────────────────────────────────────────────
const string FrontendPolicy = "FrontendPolicy";

builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendPolicy, p =>
        p.WithOrigins("http://localhost:5173")
         .AllowAnyHeader()
         .AllowAnyMethod());
});
#endregion

#region ─── Servicii & infrastructură ─────────────────────────────────────────────
builder.AddCorsConfiguration()
       .AddRepository()
       .AddAuthorizationWithSwagger("MobyLab Web App")
       .AddServices()
       .UseLogger()
       .AddWorkers();

builder.Services.AddControllers();
builder.AddApi();

// config externe
builder.Services.Configure<TMDBConfiguration>(builder.Configuration.GetSection("TMDB"));
builder.Services.Configure<MailConfiguration>(builder.Configuration.GetSection("MailConfiguration"));

// servicii domeniu
builder.Services.AddScoped<IMovieService,   MovieService>();
builder.Services.AddScoped<ICrewService,   CrewService>();
builder.Services.AddScoped<IReviewService,  ReviewService>();
builder.Services.AddScoped<ITmdbSeederService, TmdbSeederService>();
builder.Services.AddHostedService<TmdbImportWorker>();
builder.Services.AddScoped<IEventService,   EventService>();
builder.Services.AddScoped<IFriendshipService, FriendshipService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();

#endregion

var app = builder.Build();

#region ─── Seed TMDB (o singură dată) ────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<ITmdbSeederService>();
    await seeder.SeedGenresAndMoviesAsync();
}
#endregion

#region ─── Pipeline ──────────────────────────────────────────────────────────────
app.UseCors(FrontendPolicy);

app.ConfigureApplication();

app.MapControllers();
#endregion

app.Run();
