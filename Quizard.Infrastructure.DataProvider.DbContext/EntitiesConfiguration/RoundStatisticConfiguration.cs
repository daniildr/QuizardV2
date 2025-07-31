using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quizard.Core.Entities;
using Quizard.Infrastructure.DataProvider.DbContext.EntitiesConfiguration.Abstractions;

namespace Quizard.Infrastructure.DataProvider.DbContext.EntitiesConfiguration;

public class RoundStatisticConfiguration : BaseEntityConfiguration<RoundStatistic>
{
    public override void Configure(EntityTypeBuilder<RoundStatistic> builder)
    {
        builder.ToTable("RoundStatistics");
        base.Configure(builder);
        
        builder
            .HasOne(roundStatistic => roundStatistic.Game)
            .WithMany()
            .HasForeignKey(roundStatistic => roundStatistic.GameId)
            .OnDelete(DeleteBehavior.Cascade);
        builder
            .HasOne(roundStatistic => roundStatistic.Player)
            .WithMany()
            .HasForeignKey(roundStatistic => roundStatistic.PlayerId)
            .OnDelete(DeleteBehavior.Cascade);
        builder
            .HasOne(roundStatistic => roundStatistic.Round)
            .WithMany()
            .HasForeignKey(roundStatistic => roundStatistic.RoundId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Property(stage => stage.Score).IsRequired();
    }
}