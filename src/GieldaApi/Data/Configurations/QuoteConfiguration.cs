using GieldaApi.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GieldaApi.Data.Configurations;

public class QuoteConfiguration : IEntityTypeConfiguration<Quote>
{
    public void Configure(EntityTypeBuilder<Quote> builder)
    {
        builder.ToTable("Quotes");
        builder.HasKey(q => q.Id);

        builder.Property(q => q.Close).HasPrecision(18, 2);

        // Jedna spółka = maksymalnie jedno notowanie na dzień.
        builder.HasIndex(q => new { q.CompanyId, q.Date }).IsUnique();
    }
}
