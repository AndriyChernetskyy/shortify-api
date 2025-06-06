using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;


namespace Shortify.DataAccess.DataContext;

/// <summary>
/// Factory for creating ShortifyDbContext instances at design time
/// </summary>
public class ContextFactory : IDesignTimeDbContextFactory<ShortifyDbContext>
{
    private const string ApiProjectName = "Shortify.Api";
    private const string SettingsFileName = "appsettings.json";
    private const string ConnectionStringKey = "DefaultConnection";
    private const int CommandTimeoutMinutes = 10;

    public ShortifyDbContext CreateDbContext(string[] args)
    {
        ArgumentNullException.ThrowIfNull(args);

        var optionsBuilder = new DbContextOptionsBuilder<ShortifyDbContext>();
        var connectionString = GetConnectionString();
        
        optionsBuilder.UseNpgsql(connectionString, opts => 
            opts.CommandTimeout((int)TimeSpan.FromMinutes(CommandTimeoutMinutes).TotalSeconds));

        return new ShortifyDbContext(optionsBuilder.Options);
    }

    private static string GetConnectionString()
    {
        var parentDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.FullName 
                              ?? throw new InvalidOperationException("Unable to determine parent directory");
            
        var configBuilder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(parentDirectory, ApiProjectName))
            .AddJsonFile(SettingsFileName);

        var configuration = configBuilder.Build();
        
        return configuration.GetConnectionString(ConnectionStringKey) 
               ?? throw new InvalidOperationException($"Connection string '{ConnectionStringKey}' not found");
    }
}