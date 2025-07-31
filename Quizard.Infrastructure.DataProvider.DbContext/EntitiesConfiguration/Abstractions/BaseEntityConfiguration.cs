using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quizard.Core.Entities.Abstraction;
using Quizard.Infrastructure.DataProvider.DbContext.ValueConverters;

namespace Quizard.Infrastructure.DataProvider.DbContext.EntitiesConfiguration.Abstractions;

public class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(baseEntity => baseEntity.Id);
        builder.Property(entity => entity.Id)
            .HasConversion(new UlidValueConverter())
            .HasColumnType("bytea");
    }
}