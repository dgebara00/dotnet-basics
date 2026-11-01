using GieldaApi.Contracts;
using GieldaApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace GieldaApi.Controllers;

// [ApiController]: automatyczna walidacja (400), wiązanie body z JSON-a, ProblemDetails przy błędach.
[ApiController]
[Route("api/companies")]
public sealed class CompaniesController(ICompanyService companies) : ControllerBase
{
    // GET /api/companies?sector=Banki&page=1&pageSize=20
    [HttpGet]
    public async Task<ActionResult<PagedResponse<CompanyResponse>>> GetAll(
        [FromQuery] CompanyQuery query, CancellationToken ct)
        => Ok(await companies.GetAllAsync(query, ct));

    // GET /api/companies/PKO
    [HttpGet("{ticker}", Name = nameof(GetByTicker))]
    public async Task<ActionResult<CompanyDetailsResponse>> GetByTicker(string ticker, CancellationToken ct)
        => Ok(await companies.GetByTickerAsync(ticker, ct));

    // POST /api/companies
    [HttpPost]
    public async Task<ActionResult<CompanyResponse>> Create(CreateCompanyRequest request, CancellationToken ct)
    {
        var created = await companies.CreateAsync(request, ct);

        // 201 Created + nagłówek Location wskazujący na nowy zasób.
        return CreatedAtRoute(nameof(GetByTicker), new { ticker = created.Ticker }, created);
    }

    // PUT /api/companies/PKO
    [HttpPut("{ticker}")]
    public async Task<IActionResult> Update(string ticker, UpdateCompanyRequest request, CancellationToken ct)
    {
        await companies.UpdateAsync(ticker, request, ct);
        return NoContent();
    }

    // DELETE /api/companies/PKO
    [HttpDelete("{ticker}")]
    public async Task<IActionResult> Delete(string ticker, CancellationToken ct)
    {
        await companies.DeleteAsync(ticker, ct);
        return NoContent();
    }
}
