using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quizard.Core.Entities;

namespace Quizard.Infrastructure.DataProvider.DbContext.EntitiesConfiguration;

public class RoundConfiguration : IEntityTypeConfiguration<Round>
{
    public void Configure(EntityTypeBuilder<Round> builder)
    {
        builder.ToTable("Rounds");
        
        builder.Property(round => round.RoundId).IsRequired();
        builder.Property(round => round.ScenarioId).IsRequired();
        builder.Property(round => round.RoundTypeId).IsRequired();
        builder
            .HasOne(round => round.RoundType)
            .WithMany()
            .HasForeignKey(round => round.RoundTypeId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Property(round => round.Name).IsRequired(false);
        builder.Property(round => round.Description).IsRequired(false);
        builder.Property(round => round.PreviewUrl).IsRequired(false);
        builder.Property(round => round.RoundDuration).IsRequired(false);
        builder.Property(round => round.CorrectMultiplier).IsRequired();
        builder.Property(round => round.MissedMultiplier).IsRequired();
        builder.Property(round => round.IncorrectMultiplier).IsRequired();
        builder
            .HasMany(round => round.Questions)
            .WithOne()
            .HasForeignKey(question => question.RoundId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}