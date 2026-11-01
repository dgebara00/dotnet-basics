using GieldaApi.Contracts;

namespace GieldaApi.Services;

// Interfejs pozwala podmienić implementację, np. na atrapę w testach.
public interface ICompanyService
{
    Task<PagedResponse<CompanyResponse>> GetAllAsync(CompanyQuery query, CancellationToken ct);
    Task<CompanyDetailsResponse> GetByTickerAsync(string ticker, CancellationToken ct);
    Task<CompanyResponse> CreateAsync(CreateCompanyRequest request, CancellationToken ct);
    Task UpdateAsync(string ticker, UpdateCompanyRequest request, CancellationToken ct);
    Task DeleteAsync(string ticker, CancellationToken ct);
}
