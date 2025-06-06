using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shortify.Domain.Models;

namespace Shortify.DataAccess.FluentConfiguration;

public class UrlMappingConfiguration : IEntityTypeConfiguration<UrlMapping>
{
    public void Configure(EntityTypeBuilder<UrlMapping> builder)
    {
        builder.ToTable("UrlMappings")
            .HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(x => x.Url)
            .HasColumnType("varchar(2048)")
            .IsRequired();
        
        builder.Property(x => x.ShortUrl)
            .HasColumnType("varchar(21)")
            .IsRequired();
    }
}