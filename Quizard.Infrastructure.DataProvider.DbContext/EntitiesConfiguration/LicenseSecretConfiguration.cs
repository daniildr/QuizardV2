using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quizard.Core.Entities;
using Quizard.Infrastructure.DataProvider.DbContext.EntitiesConfiguration.Abstractions;

namespace Quizard.Infrastructure.DataProvider.DbContext.EntitiesConfiguration;

/// <summary> Конфигурация сущности секретов лицензии </summary>
public class LicenseSecretConfiguration : InternalBaseEntityConfiguration<LicenseSecret>
{
    public override void Configure(EntityTypeBuilder<LicenseSecret> builder)
    {
        builder.ToTable("LicenseSecrets");
        base.Configure(builder);

        builder.Property(secret => secret.Salt).IsRequired();
        builder.Property(secret => secret.CreatedAt).IsRequired();
    }
}