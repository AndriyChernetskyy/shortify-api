using Microsoft.AspNetCore.Mvc;
using Shortify.BusinessLogic.DTOs;
using Shortify.BusinessLogic.Services.Contracts;
using Shortify.BusinessLogic.Validation;

namespace Shortify.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UrlController(IUrlShortenerService urlShortenerService) : ControllerBase
{
    [HttpGet("full-url/{shortUrl}")]
    public async Task<IActionResult> GetUrl(string shortUrl)
    {
        var validator = new GetUrlValidator();
        var result = await validator.ValidateAsync(shortUrl);

        if (!result.IsValid)
        {
            return BadRequest(result.Errors);
        }
        
        var url = await urlShortenerService.GetUrl(shortUrl);

        if (url == null)
        {
            return NotFound();
        }

        return Ok(url);
    }
    
    [HttpPost("generate-short-url")]
    public async Task<IActionResult> CreateShortUrl([FromBody] CreateShortenedUrlDto createShortenedUrlDto)
    {
        var urlMapping = await urlShortenerService.CreateShortUrl(createShortenedUrlDto.Url);
        
        return Ok(urlMapping);
    }
}