using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quizard.Core.Entities;

namespace Quizard.Infrastructure.DataProvider.DbContext.EntitiesConfiguration;

public class RoundTypeConfiguration : IEntityTypeConfiguration<RoundType>
{
    public void Configure(EntityTypeBuilder<RoundType> builder)
    {
        builder.ToTable("RoundTypes");
        
        builder.Property(roundType => roundType.RoundTypeId).IsRequired();
        builder.Property(roundType => roundType.Name).IsRequired();
        builder.Property(roundType => roundType.Description).IsRequired();
        builder.Property(roundType => roundType.RoundClass).IsRequired();
        builder.Property(roundType => roundType.WaitingRoundTimeout).IsRequired();
        builder.Property(roundType => roundType.DisplayQuestionOnInformator).IsRequired();
        builder.Property(roundType => roundType.DisplayQuestionOnPlayers).IsRequired();
    }
}