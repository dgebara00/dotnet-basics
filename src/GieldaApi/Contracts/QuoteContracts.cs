using System.ComponentModel.DataAnnotations;

namespace GieldaApi.Contracts;

public sealed record QuoteResponse(
    DateOnly Date,
    decimal Close,
    long Volume);

public sealed record CreateQuoteRequest
{
    [Required]
    public required DateOnly Date { get; init; }

    // InvariantCulture: inaczej na polskim systemie "0.01" nie sparsowałoby się jako liczba.
    [Range(typeof(decimal), "0.01", "1000000",
        ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
    public required decimal Close { get; init; }

    [Range(0, long.MaxValue)]
    public required long Volume { get; init; }
}

// /api/companies/PKO/quotes?from=2026-10-01&to=2026-10-31
public sealed record QuoteQuery
{
    public DateOnly? From { get; init; }
    public DateOnly? To { get; init; }
}
