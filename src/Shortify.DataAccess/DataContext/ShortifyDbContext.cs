using Microsoft.EntityFrameworkCore;
using Shortify.DataAccess.FluentConfiguration;
using Shortify.Domain.Models;

namespace Shortify.DataAccess.DataContext;

public class ShortifyDbContext : DbContext
{
    private readonly string _connectionString;
    
    public ShortifyDbContext(DbContextOptions<ShortifyDbContext> options) : base(options)
    {
    }

    public ShortifyDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public DbSet<UrlMapping> UrlMappings { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString, options => options.EnableRetryOnFailure());
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UrlMappingConfiguration());
    }
}