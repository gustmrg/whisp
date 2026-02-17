using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Whisp.API.Middlewares;
using Whisp.Infrastructure;
using Whisp.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>
        {
            ["Bearer"] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter your JWT token"
            }
        };
        return Task.CompletedTask;
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Whisp API")
            .AddPreferredSecuritySchemes("Bearer")
            .AddHttpAuthentication("Bearer", auth =>
            {
                auth.Token = string.Empty;
            });
    });
    
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    // Retry database connection and migration
    var maxRetries = 10;
    var delaySeconds = 3;

    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            logger.LogInformation("Attempting to connect to database and apply migrations (attempt {Attempt}/{MaxRetries})", i + 1, maxRetries);
            db.Database.Migrate();
            logger.LogInformation("Database migrations applied successfully");
            break;
        }
        catch (Exception ex) when (i < maxRetries - 1)
        {
            logger.LogWarning(ex, "Failed to connect to database. Retrying in {DelaySeconds} seconds...", delaySeconds);
            await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
        }
    }
}

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}