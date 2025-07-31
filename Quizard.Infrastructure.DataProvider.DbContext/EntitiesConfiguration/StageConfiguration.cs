using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quizard.Core.Entities;
using Quizard.Infrastructure.DataProvider.DbContext.EntitiesConfiguration.Abstractions;

namespace Quizard.Infrastructure.DataProvider.DbContext.EntitiesConfiguration;

public class StageConfiguration : BaseEntityConfiguration<Stage>
{
    public override void Configure(EntityTypeBuilder<Stage> builder)
    {
        builder.ToTable("Stages");
        base.Configure(builder);
        
        builder.Property(stage => stage.ScenarioId).IsRequired();
        builder.Property(stage => stage.Index).IsRequired();
        builder.Property(stage => stage.Type).IsRequired();
        builder.Property(stage => stage.MediaId).IsRequired(false);
        builder
            .HasOne(stage => stage.Media)
            .WithMany()
            .HasForeignKey(stage => stage.MediaId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Property(stage => stage.RoundId).IsRequired(false);
        builder
            .HasOne(stage => stage.Round)
            .WithMany()
            .HasForeignKey(stage => stage.RoundId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Property(stage => stage.StageDuration).IsRequired(false);
    }
}