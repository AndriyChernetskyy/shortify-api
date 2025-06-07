namespace Shortify.BusinessLogic.Services.Contracts;

public interface IUrlShortenerService
{
    Task<string> ShortenUrl(string url);
    
    Task<string?> GetUrl(string shortUrl);
}