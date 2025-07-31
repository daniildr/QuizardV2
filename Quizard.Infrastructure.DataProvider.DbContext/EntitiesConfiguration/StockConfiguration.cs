using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quizard.Core.Entities;
using Quizard.Infrastructure.DataProvider.DbContext.EntitiesConfiguration.Abstractions;

namespace Quizard.Infrastructure.DataProvider.DbContext.EntitiesConfiguration;

public class StockConfiguration : BaseEntityConfiguration<Stock>
{
    public override void Configure(EntityTypeBuilder<Stock> builder)
    {
        builder.ToTable("Stocks");
        base.Configure(builder);
        
        builder.Property(stage => stage.ScenarioId).IsRequired();
        builder.Property(stage => stage.ModifierType).IsRequired();
        builder.Property(stage => stage.Name).IsRequired();
        builder.Property(stage => stage.Description).IsRequired();
        builder.Property(stage => stage.IconUrl).IsRequired();
        builder.Property(stage => stage.Quantity).IsRequired();
        builder.Property(stage => stage.CostMultiplier).IsRequired();
        builder.Property(stage => stage.UniqForPlayer).IsRequired();
    }
}