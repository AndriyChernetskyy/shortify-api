using Microsoft.EntityFrameworkCore;
using Shortify.DataAccess.FluentConfiguration;
using Shortify.Domain.Models;

namespace Shortify.DataAccess.DataContext;

public class ShortifyDbContext : DbContext
{
    public ShortifyDbContext(DbContextOptions<ShortifyDbContext> options) : base(options)
    {
    }

    public DbSet<UrlMapping> UrlMappings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UrlMappingConfiguration());
    }
}
