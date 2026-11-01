using GieldaApi.Common.Errors;
using GieldaApi.Data;
using GieldaApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ---------- Rejestracja usług (DI) ----------

builder.Services.AddControllers();
builder.Services.AddOpenApi();          // dokument OpenAPI pod /openapi/v1.json (do importu w Postmanie)
builder.Services.AddProblemDetails();   // spójny format błędów
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddDbContext<AppDbContext>(options => options
    .UseSqlite(builder.Configuration.GetConnectionString("Default"))
    .UseSeeding(DataSeeder.Seed)
    .UseAsyncSeeding(DataSeeder.SeedAsync));

// Scoped = jedna instancja na jedno żądanie HTTP (tak samo jak DbContext).
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IQuoteService, QuoteService>();

var app = builder.Build();

// ---------- Pipeline żądania (middleware) ----------

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Tylko lokalnie: przy starcie nakłada migracje i wrzuca dane startowe.
    // Na produkcji migracje uruchamia się osobno, np. w pipeline CI/CD.
    await using var scope = app.Services.CreateAsyncScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

app.MapControllers();

app.Run();
