using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quizard.Core.Entities;
using Quizard.Infrastructure.DataProvider.DbContext.EntitiesConfiguration.Abstractions;

namespace Quizard.Infrastructure.DataProvider.DbContext.EntitiesConfiguration;

public class LocalizationConfiguration : BaseEntityConfiguration<Localization>
{
    public override void Configure(EntityTypeBuilder<Localization> builder)
    {
        builder.ToTable("Localizations");
        base.Configure(builder);
        
        builder.Property(localization => localization.ScenarioId).IsRequired();
        builder.Property(localization => localization.ButtonUp).IsRequired();
        builder.Property(localization => localization.ButtonDown).IsRequired();
        builder.Property(localization => localization.ButtonConfirm).IsRequired();
        builder.Property(localization => localization.ButtonCancel).IsRequired();
        builder.Property(localization => localization.ButtonContinue).IsRequired();
        builder.Property(localization => localization.ButtonBuy).IsRequired();
        builder.Property(localization => localization.ButtonFinish).IsRequired();
        builder.Property(localization => localization.ButtonStub).IsRequired();
        builder.Property(localization => localization.BidButtonLabel).IsRequired();
        
        builder.Property(localization => localization.AttentionLabel).IsRequired();
        builder.Property(localization => localization.WaitOtherPlayersLabel).IsRequired();
        builder.Property(localization => localization.WaitYourTurnLabel).IsRequired();
        builder.Property(localization => localization.HintLabel).IsRequired();
        builder.Property(localization => localization.SpeedWinnerLabel).IsRequired();
        builder.Property(localization => localization.PantomimeLabel).IsRequired();
        builder.Property(localization => localization.VotingLabel).IsRequired();
        builder.Property(localization => localization.PlayerLabel).IsRequired();
        builder.Property(localization => localization.ScoreLabel).IsRequired();
        builder.Property(localization => localization.BetTime).IsRequired();
        builder.Property(localization => localization.MakeBetLabelPart1).IsRequired();
        builder.Property(localization => localization.MakeBetLabelPart2).IsRequired();
        builder.Property(localization => localization.BidAcceptedLabelPart1).IsRequired();
        builder.Property(localization => localization.BidAcceptedLabelPart2).IsRequired();
        builder.Property(localization => localization.PriceLabel).IsRequired();
        
        builder.Property(localization => localization.LoginLabel).IsRequired();
        builder.Property(localization => localization.RoundLabel).IsRequired();
        builder.Property(localization => localization.RoundResultsLabel).IsRequired();
        builder.Property(localization => localization.RoundChoiceLabel).IsRequired();
        builder.Property(localization => localization.ShopLabel).IsRequired();
        builder.Property(localization => localization.InsufficientFunds).IsRequired();
        builder.Property(localization => localization.ProductHasBeenPurchased).IsRequired();
        builder.Property(localization => localization.ProductAlreadyPurchased).IsRequired();
        builder.Property(localization => localization.ShopWaitLabel).IsRequired();
        builder.Property(localization => localization.ApplyingModifiersLabel).IsRequired();
        builder.Property(localization => localization.GameFinishLabel).IsRequired();
    }
}