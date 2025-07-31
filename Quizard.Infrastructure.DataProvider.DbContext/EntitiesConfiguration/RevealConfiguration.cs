using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quizard.Core.Entities;
using Quizard.Infrastructure.DataProvider.DbContext.EntitiesConfiguration.Abstractions;

namespace Quizard.Infrastructure.DataProvider.DbContext.EntitiesConfiguration;

public class RevealConfiguration : BaseEntityConfiguration<Reveal>
{
    public override void Configure(EntityTypeBuilder<Reveal> builder)
    {
        builder.ToTable("Reveals");
        base.Configure(builder);
        
        builder.Property(reveal => reveal.QuestionId).IsRequired();
        builder.Property(reveal => reveal.MediaId).IsRequired(false);
        builder
            .HasOne(reveal => reveal.Media)
            .WithMany()
            .HasForeignKey(reveal => reveal.MediaId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Property(reveal => reveal.Text).IsRequired();
        builder.Property(reveal => reveal.Duration).IsRequired();
    }
}