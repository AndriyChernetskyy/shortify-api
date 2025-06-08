using Microsoft.Extensions.Caching.Memory;
using Moq;
using Shortify.BusinessLogic.Services;
using Shortify.BusinessLogic.Services.Contracts; 
using Shortify.DataAccess.Repositories.Contracts;
using Shortify.Domain.Models;

namespace Shortify.BusinessLogicTests;

public class UrlShortenerServiceTests
{
    private readonly Mock<IUrlMappingRepository> _urlMappingRepositoryMock;
    private readonly Mock<IMemoryCache> _cacheMock;
    private readonly Mock<IBase62Converter> _base62ConverterMock;
    private readonly UrlShortenerService _service;

    public UrlShortenerServiceTests()
    {
        _urlMappingRepositoryMock = new Mock<IUrlMappingRepository>();
        _cacheMock = new Mock<IMemoryCache>();
        _base62ConverterMock = new Mock<IBase62Converter>();
        
        _service = new UrlShortenerService(_urlMappingRepositoryMock.Object,
            _cacheMock.Object,
            _base62ConverterMock.Object);
    }

    [Fact]
    public async Task ShortenUrl_WhenUrlExists_ReturnsExistingShortUrl()
    {
        // Arrange
        const string originalUrl = "https://example.com";
        const string existingShortUrl = "abcd";
        var urlMapping = new UrlMapping 
        { 
            Url = originalUrl, 
            ShortUrl = existingShortUrl 
        };

        _urlMappingRepositoryMock
            .Setup(x => x.GetUrlMapping(originalUrl))
            .ReturnsAsync(urlMapping);
        _cacheMock
            .Setup(x => x.CreateEntry(It.IsAny<object>()))
            .Callback<object>(key => urlMapping.ShortUrl = key.ToString())
            .Returns(Mock.Of<ICacheEntry>());

        // Act
        var result = await _service.CreateShortUrl(originalUrl);

        // Assert
        Assert.Equal(existingShortUrl, result.ShortUrl);;
        _urlMappingRepositoryMock.Verify(x => x.CreateShortUrl(It.IsAny<UrlMapping>()), Times.Never);
    }

    [Fact]
    public async Task ShortenUrl_WhenUrlDoesNotExist_CreatesNewShortUrl()
    {
        // Arrange
        var originalUrl = "https://example.com";
        var newId = 12345;
        var shortUrlCode = "b3k5";
        var expectedShortUrl = $"{shortUrlCode}";

        _urlMappingRepositoryMock
            .Setup(x => x.GetUrlMapping(originalUrl))
            .ReturnsAsync((UrlMapping)null);

        _urlMappingRepositoryMock
            .Setup(x => x.GetNextUrlIdAsync())
            .ReturnsAsync(newId);

        _base62ConverterMock
            .Setup(x => x.Encode(newId, 4))
            .Returns(shortUrlCode);

        _cacheMock
            .Setup(x => x.CreateEntry(It.IsAny<object>()))
            .Callback<object>(key => expectedShortUrl = key.ToString())
            .Returns(Mock.Of<ICacheEntry>());

        // Act
        var result = await _service.CreateShortUrl(originalUrl);

        // Assert
        Assert.Equal(expectedShortUrl, result.ShortUrl);
    
        _urlMappingRepositoryMock.Verify(x => x.GetNextUrlIdAsync(), Times.Once);
        _urlMappingRepositoryMock.Verify(x => x.CreateShortUrl(
                It.Is<UrlMapping>(m => 
                    m.Url == originalUrl && 
                    m.ShortUrl == expectedShortUrl)), 
            Times.Once);
    }

    [Theory]
    [InlineData(1, "a")]
    [InlineData(62, "10")]
    [InlineData(12345, "b3k5")]
    public async Task ShortenUrl_WithDifferentIds_ReturnsExpectedUrls(int id, string encodedValue)
    {
        // Arrange
        var expectedUrl = $"{encodedValue}";
        
        _urlMappingRepositoryMock
            .Setup(x => x.GetUrlMapping(It.IsAny<string>()))
            .ReturnsAsync((UrlMapping)null);

        _urlMappingRepositoryMock
            .Setup(x => x.GetNextUrlIdAsync())
            .ReturnsAsync(id);

        _base62ConverterMock
            .Setup(x => x.Encode(id, 4))
            .Returns(encodedValue);
        
        _cacheMock
            .Setup(x => x.CreateEntry(It.IsAny<object>()))
            .Callback<object>(key => expectedUrl = key.ToString())
            .Returns(Mock.Of<ICacheEntry>());

        // Act
        var result = await _service.CreateShortUrl("https://example.com");

        // Assert
        Assert.Equal(expectedUrl, result.ShortUrl);;
        _base62ConverterMock.Verify(x => x.Encode(id, 4), Times.Once);
    }

    [Fact] 
    public async Task GenerateUniqueShortUrl_UsesDefaultMaxLength()
    {
        const int id = 12345;
        const string encodedValue = "b3k5";
        const int expectedMaxLength = 4;
        var expectedUrl = $"{encodedValue}";

        var mapping = new UrlMapping
        {
            Id = id,
            Url = "https://example.com",
            ShortUrl = encodedValue
        };
        
        _urlMappingRepositoryMock
            .Setup(x => x.GetUrlMapping(It.IsAny<string>()))
            .ReturnsAsync((UrlMapping)null);
        _urlMappingRepositoryMock
            .Setup(x => x.GetNextUrlIdAsync())
            .ReturnsAsync(id);
        _urlMappingRepositoryMock
            .Setup(x => x.CreateShortUrl(It.IsAny<UrlMapping>()))
            .Returns(Task.FromResult(mapping));
        _base62ConverterMock
            .Setup(x => x.Encode(id, expectedMaxLength))
            .Returns(encodedValue);
        _cacheMock
            .Setup(x => x.CreateEntry(It.IsAny<object>()))
            .Callback<object>(key => expectedUrl = key.ToString())
            .Returns(Mock.Of<ICacheEntry>());

        // Act
        var result = await _service.CreateShortUrl("https://example.com");

        // Assert
        Assert.Equal(mapping, result);
        _base62ConverterMock.Verify(x => x.Encode(id, expectedMaxLength), Times.Once);
        _urlMappingRepositoryMock.Verify(x => x.GetUrlMapping(It.IsAny<string>()), Times.Once);
        _urlMappingRepositoryMock.Verify(x => x.GetNextUrlIdAsync(), Times.Once);
        _urlMappingRepositoryMock.Verify(x => x.CreateShortUrl(It.IsAny<UrlMapping>()), Times.Once);;
    }
}