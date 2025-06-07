using Microsoft.Extensions.Caching.Memory;
using Shortify.BusinessLogic.Services.Contracts;
using Shortify.DataAccess.Repositories.Contracts;
using Shortify.Domain.Models;

namespace Shortify.BusinessLogic.Services;

public class UrlShortenerService(IUrlMappingRepository urlMappingRepository, IMemoryCache cache, IBase62Converter base62Converter) : IUrlShortenerService
{
    private const int MAX_URL_LENGTH = 4;

    public async Task<string> ShortenUrl(string url)
    {
        var urlMapping = await urlMappingRepository.GetUrlMapping(url);

        if (urlMapping != null)
        {
            return urlMapping.ShortUrl;
        }

        var shortUrl = await GenerateUniqueShortUrl();

        await urlMappingRepository.AddUrlMapping(new UrlMapping
        {
            ShortUrl = shortUrl,
            Url = url
        });

        cache.Set(shortUrl, url);

        return shortUrl;
    }
    
    public async Task<string?> GetUrl(string shortUrl)
    {
        return await urlMappingRepository.GetUrl(shortUrl);
    }

    private async Task<string> GenerateUniqueShortUrl(int maxLength = MAX_URL_LENGTH)
    {
        var id = await urlMappingRepository.GetNextUrlIdAsync();
        return base62Converter.Encode(id, maxLength);
    }
}