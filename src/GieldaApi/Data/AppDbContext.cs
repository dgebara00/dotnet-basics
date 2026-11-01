using GieldaApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace GieldaApi.Data;

// Primary constructor (C# 12) – opcje przychodzą z DI, skonfigurowane w Program.cs.
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Quote> Quotes => Set<Quote>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Wczytuje wszystkie klasy IEntityTypeConfiguration z tego projektu (folder Configurations/).
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
