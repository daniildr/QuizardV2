using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quizard.Core.Entities;
using Quizard.Infrastructure.DataProvider.DbContext.EntitiesConfiguration.Abstractions;

namespace Quizard.Infrastructure.DataProvider.DbContext.EntitiesConfiguration;

public class PlayerConfiguration : BaseEntityConfiguration<Player>
{
    public override void Configure(EntityTypeBuilder<Player> builder)
    {
        builder.ToTable("Players");
        base.Configure(builder);
        
        builder.Property(player => player.Nickname).IsRequired().HasMaxLength(255);
        builder.HasIndex(player => player.Nickname).IsUnique();
    }
}