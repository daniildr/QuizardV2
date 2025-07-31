using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quizard.Core.Entities;
using Quizard.Infrastructure.DataProvider.DbContext.EntitiesConfiguration.Abstractions;

namespace Quizard.Infrastructure.DataProvider.DbContext.EntitiesConfiguration;

/// <summary> Конфигурация сущности лицензии </summary>
public class LicenseConfiguration : InternalBaseEntityConfiguration<License>
{
    public override void Configure(EntityTypeBuilder<License> builder)
    {
        builder.ToTable("Licenses");
        base.Configure(builder);

        builder.Property(license => license.LicenseKey).IsRequired();
        builder.Property(license => license.ExpirationTime).IsRequired();
        builder.Property(license => license.GamesLeft).IsRequired();
        builder.Property(license => license.Active).IsRequired();
        builder
            .HasOne(license => license.Salt)
            .WithOne(secret => secret.License)
            .HasForeignKey<License>(license => license.SaltId)
            .IsRequired();
    }
}