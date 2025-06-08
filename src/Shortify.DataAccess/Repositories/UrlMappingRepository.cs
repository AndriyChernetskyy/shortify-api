using Microsoft.EntityFrameworkCore;
using Shortify.DataAccess.DataContext;
using Shortify.DataAccess.Repositories.Contracts;
using Shortify.Domain.Models;

namespace Shortify.DataAccess.Repositories;

public class UrlMappingRepository(ShortifyDbContext dbContext) : IUrlMappingRepository
{
    public async Task<string?> GetUrl(string shortUrl)
    {
        var result = await dbContext.UrlMappings.FirstOrDefaultAsync(x => x.ShortUrl == shortUrl);
        
        return result?.Url;
    }

    public async Task<UrlMapping> CreateShortUrl(UrlMapping urlMapping)
    {
        await dbContext.AddAsync(urlMapping);
        
        await dbContext.SaveChangesAsync();

        return urlMapping;
    }
    
    public async Task<bool> ShortUrlExists(string shortUrl)
    {
        return await dbContext.UrlMappings.AnyAsync(x => x.ShortUrl == shortUrl);
    }
    
    public async Task<UrlMapping?> GetUrlMapping(string url)
    {
        return await dbContext.UrlMappings.FirstOrDefaultAsync(x => x.Url == url);   
    }
    
    public async Task<int> GetNextUrlIdAsync()
    {
        var connection = dbContext.Database.GetDbConnection();

        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT nextval('url_id_seq')";

        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

}