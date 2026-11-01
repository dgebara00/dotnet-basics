namespace GieldaApi.Domain;

// Jedno dzienne notowanie spółki.
public class Quote
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    public DateOnly Date { get; set; }
    public decimal Close { get; set; }
    public long Volume { get; set; }
}
