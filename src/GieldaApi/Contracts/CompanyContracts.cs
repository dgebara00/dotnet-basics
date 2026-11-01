using System.ComponentModel.DataAnnotations;

namespace GieldaApi.Contracts;

// DTO = kształt danych na wejściu i wyjściu API. Odpowiednik typów propsów/odpowiedzi w TS.
// [ApiController] waliduje atrybuty automatycznie i zwraca 400 z listą błędów.

public sealed record CompanyResponse(
    string Ticker,
    string Name,
    string Sector,
    DateTime CreatedAtUtc);

public sealed record CompanyDetailsResponse(
    string Ticker,
    string Name,
    string Sector,
    DateTime CreatedAtUtc,
    int QuotesCount,
    QuoteResponse? LastQuote);

public sealed record CreateCompanyRequest
{
    [Required]
    // Małe litery są OK – serwis i tak zamienia ticker na wielkie.
    [RegularExpression("^[A-Za-z0-9]{2,10}$", ErrorMessage = "Ticker: 2–10 liter lub cyfr.")]
    public required string Ticker { get; init; }

    [Required, StringLength(200, MinimumLength = 2)]
    public required string Name { get; init; }

    [Required, StringLength(100, MinimumLength = 2)]
    public required string Sector { get; init; }
}

public sealed record UpdateCompanyRequest
{
    [Required, StringLength(200, MinimumLength = 2)]
    public required string Name { get; init; }

    [Required, StringLength(100, MinimumLength = 2)]
    public required string Sector { get; init; }
}

// Parametry z query stringa: /api/companies?sector=Banki&page=1&pageSize=20
public sealed record CompanyQuery
{
    public string? Sector { get; init; }

    [Range(1, int.MaxValue)]
    public int Page { get; init; } = 1;

    [Range(1, 100)]
    public int PageSize { get; init; } = 20;
}
