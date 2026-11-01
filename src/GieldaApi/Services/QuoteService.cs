using GieldaApi.Common.Errors;
using GieldaApi.Contracts;
using GieldaApi.Data;
using GieldaApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace GieldaApi.Services;

public sealed class QuoteService(AppDbContext db) : IQuoteService
{
    public async Task<IReadOnlyList<QuoteResponse>> GetForCompanyAsync(
        string ticker, QuoteQuery query, CancellationToken ct)
    {
        var companyId = await GetCompanyIdAsync(ticker, ct);

        var quotes = db.Quotes.AsNoTracking().Where(q => q.CompanyId == companyId);

        if (query.From is { } fromDate) quotes = quotes.Where(q => q.Date >= fromDate);
        if (query.To is { } toDate) quotes = quotes.Where(q => q.Date <= toDate);

        return await quotes
            .OrderBy(q => q.Date)
            .Select(q => new QuoteResponse(q.Date, q.Close, q.Volume))
            .ToListAsync(ct);
    }

    public async Task<QuoteResponse> AddAsync(string ticker, CreateQuoteRequest request, CancellationToken ct)
    {
        var companyId = await GetCompanyIdAsync(ticker, ct);

        if (await db.Quotes.AnyAsync(q => q.CompanyId == companyId && q.Date == request.Date, ct))
        {
            throw new ConflictException($"Notowanie z dnia {request.Date:yyyy-MM-dd} już istnieje.");
        }

        var quote = new Quote
        {
            CompanyId = companyId,
            Date = request.Date,
            Close = request.Close,
            Volume = request.Volume,
        };

        db.Quotes.Add(quote);
        await db.SaveChangesAsync(ct);

        return new QuoteResponse(quote.Date, quote.Close, quote.Volume);
    }

    private async Task<int> GetCompanyIdAsync(string ticker, CancellationToken ct)
    {
        var normalized = ticker.Trim().ToUpperInvariant();

        // Pobieramy tylko Id, a nie całą encję.
        var id = await db.Companies
            .Where(c => c.Ticker == normalized)
            .Select(c => (int?)c.Id)
            .FirstOrDefaultAsync(ct);

        return id ?? throw new NotFoundException($"Spółka {normalized} nie istnieje.");
    }
}
