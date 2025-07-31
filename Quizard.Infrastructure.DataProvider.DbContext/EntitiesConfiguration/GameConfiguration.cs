using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quizard.Core.Entities;
using Quizard.Infrastructure.DataProvider.DbContext.EntitiesConfiguration.Abstractions;

namespace Quizard.Infrastructure.DataProvider.DbContext.EntitiesConfiguration;

public class GameConfiguration : BaseEntityConfiguration<Game>
{
    public override void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.ToTable("Games");
        base.Configure(builder);
        
        builder.Property(game => game.IsRunning).IsRequired();
        builder.Property(game => game.CreatedAt).IsRequired();
        builder.Property(game => game.ScenarioId).IsRequired();
    }
}