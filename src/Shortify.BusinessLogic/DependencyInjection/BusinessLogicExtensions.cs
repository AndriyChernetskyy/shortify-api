using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Shortify.BusinessLogic.Services;
using Shortify.BusinessLogic.Services.Contracts;
using Shortify.BusinessLogic.Validation;

namespace Shortify.BusinessLogic.DependencyInjection;

public static class BusinessLogicExtensions
{
    public static IServiceCollection RegisterBusinessLogicDependencies(this IServiceCollection services)
    {
        services.RegisterServices();
        services.RegisterValidators();

        return services;
    }

    private static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IBase62Converter, Base62Converter>();
        services.AddScoped<IUrlShortenerService, UrlShortenerService>();

        return services;
    }

    private static IServiceCollection RegisterValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateShortenedUrlDtoValidator>();

        return services;
    }
}