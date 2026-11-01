using GieldaApi.Contracts;

namespace GieldaApi.Services;

public interface IQuoteService
{
    Task<IReadOnlyList<QuoteResponse>> GetForCompanyAsync(string ticker, QuoteQuery query, CancellationToken ct);
    Task<QuoteResponse> AddAsync(string ticker, CreateQuoteRequest request, CancellationToken ct);
}
