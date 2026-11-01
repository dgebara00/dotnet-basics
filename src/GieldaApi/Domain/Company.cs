namespace GieldaApi.Domain;

// Encja = to, co leży w bazie. Nigdy nie zwracamy jej wprost z API (od tego są DTO w Contracts/).
public class Company
{
    public int Id { get; set; }
    public required string Ticker { get; set; }
    public required string Name { get; set; }
    public required string Sector { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public List<Quote> Quotes { get; set; } = [];
}
