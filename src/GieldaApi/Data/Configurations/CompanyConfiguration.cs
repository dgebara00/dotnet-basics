using GieldaApi.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GieldaApi.Data.Configurations;

// Konfiguracja tabeli trzymana osobno, zamiast atrybutów na encji – encja zostaje czysta.
public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Companies");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Ticker).HasMaxLength(10).IsRequired();
        builder.Property(c => c.Name).HasMaxLength(200).IsRequired();
        builder.Property(c => c.Sector).HasMaxLength(100).IsRequired();

        // Ticker musi być unikalny – pilnuje tego baza, nie tylko kod.
        builder.HasIndex(c => c.Ticker).IsUnique();

        builder.HasMany(c => c.Quotes)
            .WithOne(q => q.Company)
            .HasForeignKey(q => q.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
