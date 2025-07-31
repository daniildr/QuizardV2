using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quizard.Core.Entities.Abstraction;

namespace Quizard.Infrastructure.DataProvider.DbContext.EntitiesConfiguration.Abstractions;

public abstract class InternalBaseEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : InternalBaseEntity
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(internalBaseEntity => internalBaseEntity.Id);
        builder.Property(internalBaseEntity => internalBaseEntity.Id).ValueGeneratedOnAdd();
    }
}