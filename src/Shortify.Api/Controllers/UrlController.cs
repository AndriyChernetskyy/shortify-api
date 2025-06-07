using Microsoft.AspNetCore.Mvc;
using Shortify.BusinessLogic.DTOs;
using Shortify.BusinessLogic.Services.Contracts;

namespace Shortify.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UrlController(IUrlShortenerService urlShortenerService) : ControllerBase
{
    [HttpGet("redirect")]
    public async Task<IActionResult> GetUrl([FromBody] GetUrlDto getUrlDto)
    {
        var url = await urlShortenerService.GetUrl(getUrlDto.ShortUrl);

        if (url == null)
        {
            return NotFound();
        }

        return RedirectPermanent(url);
    }
    
    [HttpPost("generate-short-url")]
    public async Task<IActionResult> CreateShortUrl([FromBody] CreateShortenedUrlDto createShortenedUrlDto)
    {
        var shortUrl = await urlShortenerService.ShortenUrl(createShortenedUrlDto.Url);
        
        return Ok(shortUrl);
    }
}