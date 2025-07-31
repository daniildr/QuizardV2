using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quizard.Core.Entities;

namespace Quizard.Infrastructure.DataProvider.DbContext.EntitiesConfiguration;

public class MediaConfiguration : IEntityTypeConfiguration<Media>
{
    public void Configure(EntityTypeBuilder<Media> builder)
    {
        builder.ToTable("Media");
        
        builder.Property(media => media.MediaId).IsRequired();
        builder.Property(media => media.Type).IsRequired();
        builder.Property(media => media.Url).IsRequired(false);
        builder.Property(media => media.Content).IsRequired(false);
        builder.Property(media => media.Content).IsRequired(false);
        builder.Property(media => media.DelaySeconds).IsRequired();
        builder.Property(media => media.Duration).IsRequired(false);
        builder.Property(media => media.ShowOnPlayer).IsRequired();
    }
}