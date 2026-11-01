namespace GieldaApi.Contracts;

// Wspólny kształt odpowiedzi ze stronicowaniem – generyk jak PagedResponse<T> w TS.
public sealed record PagedResponse<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalCount)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
