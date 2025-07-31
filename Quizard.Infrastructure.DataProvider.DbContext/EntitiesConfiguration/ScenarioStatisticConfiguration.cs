using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quizard.Core.Entities;
using Quizard.Infrastructure.DataProvider.DbContext.EntitiesConfiguration.Abstractions;

namespace Quizard.Infrastructure.DataProvider.DbContext.EntitiesConfiguration;

public class ScenarioStatisticConfiguration : BaseEntityConfiguration<ScenarioStatistic>
{
    public override void Configure(EntityTypeBuilder<ScenarioStatistic> builder)
    {
        builder.ToTable("ScenarioStatistics");
        base.Configure(builder);
        
        builder
            .HasOne(scenarioStatistic => scenarioStatistic.Game)
            .WithMany()
            .HasForeignKey(roundStatistic => roundStatistic.GameId)
            .OnDelete(DeleteBehavior.Cascade);
        builder
            .HasOne(scenarioStatistic => scenarioStatistic.Player)
            .WithMany()
            .HasForeignKey(roundStatistic => roundStatistic.PlayerId)
            .OnDelete(DeleteBehavior.Cascade);
        builder
            .HasOne(scenarioStatistic => scenarioStatistic.Scenario)
            .WithMany()
            .HasForeignKey(scenarioStatistic => scenarioStatistic.ScenarioId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Property(stage => stage.Score).IsRequired();
    }
}