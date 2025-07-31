using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quizard.Core.Entities;

namespace Quizard.Infrastructure.DataProvider.DbContext.EntitiesConfiguration;

public class ScenarioConfiguration : IEntityTypeConfiguration<Scenario>
{
    public void Configure(EntityTypeBuilder<Scenario> builder)
    {
        builder.ToTable("Scenarios");
        
        builder.Property(scenario => scenario.Id).IsRequired();
        builder.Property(scenario => scenario.Name).IsRequired();
        builder.Property(scenario => scenario.GameDuration).IsRequired(false);
        builder
            .HasMany(scenario => scenario.Stages)
            .WithOne()
            .HasForeignKey(stage => stage.ScenarioId)
            .OnDelete(DeleteBehavior.Cascade);
        builder
            .HasMany(scenario => scenario.RoundDefinitions)
            .WithOne()
            .HasForeignKey(round => round.ScenarioId)
            .OnDelete(DeleteBehavior.Cascade);
        builder
            .HasMany(scenario => scenario.ShopStocks)
            .WithOne()
            .HasForeignKey(stock => stock.ScenarioId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Property(scenario => scenario.BasePointPrice).IsRequired();
        builder.Property(scenario => scenario.StartPlayerScore).IsRequired();
        builder.Property(scenario => scenario.RoundPresentationDuration).IsRequired(false);
        builder.Property(scenario => scenario.ShowScenarioStatsOnFinish).IsRequired();
        builder.Property(scenario => scenario.FinishPlaceholder).IsRequired(false);
        builder.Property(scenario => scenario.Placeholder).IsRequired();
        builder
            .HasOne(scenario => scenario.Localization)
            .WithOne()
            .HasForeignKey<Localization>(localization => localization.ScenarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}