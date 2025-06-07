using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shortify.DataAccess.DataContext;
using Shortify.DataAccess.Repositories;
using Shortify.DataAccess.Repositories.Contracts;

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
        services.AddTransient<IUrlMappingRepository, UrlMappingRepository>();
    }

    private static void MigrateDatabase(string connectionString)
    {
        var options = new DbContextOptionsBuilder<ShortifyDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        using var context = new ShortifyDbContext(options);
        context.Database.Migrate();
    }

}