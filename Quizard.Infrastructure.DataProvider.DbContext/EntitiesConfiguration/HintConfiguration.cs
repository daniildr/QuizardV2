using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quizard.Core.Entities;
using Quizard.Infrastructure.DataProvider.DbContext.EntitiesConfiguration.Abstractions;

namespace Quizard.Infrastructure.DataProvider.DbContext.EntitiesConfiguration;

public class HintConfiguration : BaseEntityConfiguration<Hint>
{
    public override void Configure(EntityTypeBuilder<Hint> builder)
    {
        builder.ToTable("Hints");
        base.Configure(builder);
        
        builder.Property(round => round.QuestionId).IsRequired();
        builder.Property(round => round.Text).IsRequired();
    }
}