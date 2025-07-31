using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quizard.Core.Entities;
using Quizard.Infrastructure.DataProvider.DbContext.EntitiesConfiguration.Abstractions;

namespace Quizard.Infrastructure.DataProvider.DbContext.EntitiesConfiguration;

public class AnswerConfiguration : BaseEntityConfiguration<Answer>
{
    public override void Configure(EntityTypeBuilder<Answer> builder)
    {
        builder.ToTable("Answers");
        base.Configure(builder);
        
        builder.Property(answer => answer.QuestionId).IsRequired();
        builder.Property(answer => answer.Text).IsRequired();
        builder.Property(answer => answer.Button).IsRequired();
        builder.Property(answer => answer.IsCorrect).IsRequired(false);
        builder.Property(answer => answer.Order).IsRequired(false);
    }
}