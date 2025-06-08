using Shortify.Domain.Models;

namespace Shortify.DataAccess.Repositories.Contracts;

public interface IUrlMappingRepository
{
    Task<string?> GetUrl(string shortUrl);

    Task<UrlMapping> CreateShortUrl(UrlMapping urlMapping);

    Task<bool> ShortUrlExists(string shortUrl);

    Task<UrlMapping?> GetUrlMapping(string url);

    Task<int> GetNextUrlIdAsync();
}