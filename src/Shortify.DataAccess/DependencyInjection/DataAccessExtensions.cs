using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shortify.DataAccess.DataContext;

namespace Shortify.DataAccess.DependencyInjection;

public static class DataAccessExtensions
{
    public static IServiceCollection RegisterDataAccessDependencies(this IServiceCollection services, string connectionString)
    {
        services.RegisterRepositories();
        
        MigrateDatabase(connectionString);
        
        return services;
    }

    private static void RegisterRepositories(this IServiceCollection services)
    {
    }

    private static void MigrateDatabase(string connectionString)
    {
        using ShortifyDbContext dbContext = new(connectionString);
        if (dbContext.Database.GetMigrations().Any())
        {
            dbContext.Database.Migrate();
        }
    }
}