using GieldaApi.Common.Errors;
using GieldaApi.Contracts;
using GieldaApi.Data;
using GieldaApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace GieldaApi.Services;

// Logika biznesowa i dostęp do danych. Kontroler tylko przyjmuje żądanie i woła serwis.
public sealed class CompanyService(AppDbContext db) : ICompanyService
{
    public async Task<PagedResponse<CompanyResponse>> GetAllAsync(CompanyQuery query, CancellationToken ct)
    {
        // IQueryable = zapytanie jeszcze NIE wykonane. Doklejamy warunki, a EF zamieni całość na jeden SQL.
        var companies = db.Companies.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Sector))
        {
            companies = companies.Where(c => c.Sector == query.Sector);
        }

        var totalCount = await companies.CountAsync(ct);

        var items = await companies
            .OrderBy(c => c.Ticker)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            // Projekcja do DTO w zapytaniu – baza zwraca tylko potrzebne kolumny.
            .Select(c => new CompanyResponse(c.Ticker, c.Name, c.Sector, c.CreatedAtUtc))
            .ToListAsync(ct);

        return new PagedResponse<CompanyResponse>(items, query.Page, query.PageSize, totalCount);
    }

    public async Task<CompanyDetailsResponse> GetByTickerAsync(string ticker, CancellationToken ct)
    {
        var normalized = Normalize(ticker);

        var company = await db.Companies
            .AsNoTracking()
            .Where(c => c.Ticker == normalized)
            .Select(c => new { c.Id, c.Ticker, c.Name, c.Sector, c.CreatedAtUtc, QuotesCount = c.Quotes.Count })
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException($"Spółka {normalized} nie istnieje.");

        var lastQuote = await db.Quotes
            .AsNoTracking()
            .Where(q => q.CompanyId == company.Id)
            .OrderByDescending(q => q.Date)
            .Select(q => new QuoteResponse(q.Date, q.Close, q.Volume))
            .FirstOrDefaultAsync(ct);

        return new CompanyDetailsResponse(
            company.Ticker, company.Name, company.Sector, company.CreatedAtUtc,
            company.QuotesCount, lastQuote);
    }

    public async Task<CompanyResponse> CreateAsync(CreateCompanyRequest request, CancellationToken ct)
    {
        var ticker = Normalize(request.Ticker);

        if (await db.Companies.AnyAsync(c => c.Ticker == ticker, ct))
        {
            throw new ConflictException($"Spółka {ticker} już istnieje.");
        }

        var company = new Company
        {
            Ticker = ticker,
            Name = request.Name.Trim(),
            Sector = request.Sector.Trim(),
            CreatedAtUtc = DateTime.UtcNow,
        };

        db.Companies.Add(company);
        await db.SaveChangesAsync(ct);

        return new CompanyResponse(company.Ticker, company.Name, company.Sector, company.CreatedAtUtc);
    }

    public async Task UpdateAsync(string ticker, UpdateCompanyRequest request, CancellationToken ct)
    {
        // Bez AsNoTracking: EF śledzi encję i sam wykryje zmiany przy SaveChanges.
        var company = await FindTrackedAsync(ticker, ct);

        company.Name = request.Name.Trim();
        company.Sector = request.Sector.Trim();

        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(string ticker, CancellationToken ct)
    {
        var company = await FindTrackedAsync(ticker, ct);

        db.Companies.Remove(company);
        await db.SaveChangesAsync(ct);
    }

    private async Task<Company> FindTrackedAsync(string ticker, CancellationToken ct)
    {
        var normalized = Normalize(ticker);

        return await db.Companies.FirstOrDefaultAsync(c => c.Ticker == normalized, ct)
            ?? throw new NotFoundException($"Spółka {normalized} nie istnieje.");
    }

    private static string Normalize(string ticker) => ticker.Trim().ToUpperInvariant();
}
