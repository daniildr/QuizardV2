using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quizard.Core.Entities;
using Quizard.Infrastructure.DataProvider.DbContext.EntitiesConfiguration.Abstractions;

namespace Quizard.Infrastructure.DataProvider.DbContext.EntitiesConfiguration;

public class QuestionConfiguration : BaseEntityConfiguration<Question>
{
    public override void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable("Questions");
        base.Configure(builder);
        
        builder.Property(question => question.RoundId).IsRequired();
        builder.Property(question => question.QuestionNumber).IsRequired();
        builder.Property(question => question.QuestionText).IsRequired();
        builder.Property(question => question.MediaId).IsRequired(false);
        builder
            .HasOne(question => question.Media)
            .WithMany()
            .HasForeignKey(question => question.MediaId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Property(question => question.AnswerDelay).IsRequired(false);
        builder.Property(question => question.QuestionTimeout).IsRequired();
        builder
            .HasMany(question => question.Answers)
            .WithOne()
            .HasForeignKey(answer => answer.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);
        builder
            .HasOne(question => question.Reveal)
            .WithOne()
            .HasForeignKey<Reveal>(reveal => reveal.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);
        builder
            .HasMany(question => question.Hints)
            .WithOne()
            .HasForeignKey(hint => hint.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}