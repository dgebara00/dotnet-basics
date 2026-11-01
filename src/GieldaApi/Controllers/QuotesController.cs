using GieldaApi.Contracts;
using GieldaApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace GieldaApi.Controllers;

// Zasób zagnieżdżony: notowania zawsze należą do konkretnej spółki.
[ApiController]
[Route("api/companies/{ticker}/quotes")]
public sealed class QuotesController(IQuoteService quotes) : ControllerBase
{
    // GET /api/companies/PKO/quotes?from=2026-10-01&to=2026-10-31
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<QuoteResponse>>> GetAll(
        string ticker, [FromQuery] QuoteQuery query, CancellationToken ct)
        => Ok(await quotes.GetForCompanyAsync(ticker, query, ct));

    // POST /api/companies/PKO/quotes
    [HttpPost]
    public async Task<ActionResult<QuoteResponse>> Add(
        string ticker, CreateQuoteRequest request, CancellationToken ct)
    {
        var created = await quotes.AddAsync(ticker, request, ct);
        return StatusCode(StatusCodes.Status201Created, created);
    }
}
