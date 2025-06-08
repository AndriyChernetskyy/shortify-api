using Microsoft.Extensions.Caching.Memory;
using Shortify.BusinessLogic.Services.Contracts;
using Shortify.DataAccess.Repositories.Contracts;
using Shortify.Domain.Models;

namespace Shortify.BusinessLogic.Services;

public class UrlShortenerService(IUrlMappingRepository urlMappingRepository, IMemoryCache cache, IBase62Converter base62Converter) : IUrlShortenerService
{
    private const int MaxLenght = 4;

    public async Task<UrlMapping> CreateShortUrl(string url)
    {
        var urlMapping = await urlMappingRepository.GetUrlMapping(url);

        if (urlMapping != null)
        {
            return urlMapping;
        }

        var shortUrl = await GenerateUniqueShortUrl();

        var newUrlMapping = await urlMappingRepository.CreateShortUrl(new UrlMapping
        {
            ShortUrl = shortUrl,
            Url = url
        });

        cache.Set(shortUrl, url);

        return newUrlMapping;
    }
    
    public async Task<string?> GetUrl(string shortUrl)
    {
        return await urlMappingRepository.GetUrl(shortUrl);
    }

    private async Task<string> GenerateUniqueShortUrl()
    {
        var id = await urlMappingRepository.GetNextUrlIdAsync();
        return base62Converter.Encode(id, MaxLenght);
    }
}