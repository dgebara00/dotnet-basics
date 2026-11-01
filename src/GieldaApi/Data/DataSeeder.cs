using GieldaApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace GieldaApi.Data;

// Dane startowe. Liczby są PRZYKŁADOWE (zmyślone).
// Podpięte w Program.cs przez UseSeeding / UseAsyncSeeding (EF Core 9+),
// uruchamiają się automatycznie przy migracji, tylko gdy tabela jest pusta.
public static class DataSeeder
{
    public static void Seed(DbContext context, bool _)
    {
        if (context.Set<Company>().Any()) return;
        context.Set<Company>().AddRange(CreateCompanies());
        context.SaveChanges();
    }

    public static async Task SeedAsync(DbContext context, bool _, CancellationToken ct)
    {
        if (await context.Set<Company>().AnyAsync(ct)) return;
        context.Set<Company>().AddRange(CreateCompanies());
        await context.SaveChangesAsync(ct);
    }

    private static List<Company> CreateCompanies()
    {
        var now = DateTime.UtcNow;

        return
        [
            Create("PKO", "PKO Bank Polski", "Banki", now, [60.00m, 61.20m, 63.00m]),
            Create("PEO", "Bank Pekao", "Banki", now, [162.00m, 164.50m, 165.00m]),
            Create("PZU", "PZU", "Ubezpieczenia", now, [57.10m, 57.90m, 58.20m]),
            Create("PKN", "Orlen", "Paliwa", now, [70.00m, 71.40m, 72.10m]),
            Create("CDR", "CD Projekt", "Gry", now, [220.00m, 231.00m, 246.40m]),
            Create("DNP", "Dino Polska", "Handel", now, [420.00m, 415.00m, 409.50m]),
            Create("LPP", "LPP", "Handel", now, [16500.00m, 16650.00m, 16800.00m]),
            Create("KTY", "Grupa Kęty", "Przemysł", now, []),
        ];
    }

    private static Company Create(string ticker, string name, string sector, DateTime now, decimal[] closes)
    {
        var company = new Company { Ticker = ticker, Name = name, Sector = sector, CreatedAtUtc = now };

        var start = new DateOnly(2026, 10, 1);
        for (var i = 0; i < closes.Length; i++)
        {
            company.Quotes.Add(new Quote
            {
                Date = start.AddDays(i),
                Close = closes[i],
                Volume = 1_000_000 + i * 150_000,
            });
        }

        return company;
    }
}
