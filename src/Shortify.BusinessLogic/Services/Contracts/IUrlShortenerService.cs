using Shortify.Domain.Models;

namespace Shortify.BusinessLogic.Services.Contracts;

public interface IUrlShortenerService
{
    Task<UrlMapping> CreateShortUrl(string url);
    
    Task<string?> GetUrl(string shortUrl);
}