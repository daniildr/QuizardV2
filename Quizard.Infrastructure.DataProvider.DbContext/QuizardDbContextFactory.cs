using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Quizard.Infrastructure.DataProvider.DbContext;

public class QuizardDbContextFactory : IDesignTimeDbContextFactory<QuizardDbContext>
{
    public QuizardDbContext CreateDbContext(string[] args)
    {
        var configuration = Core.Utils.ConfigurationBuilder.BuildConfiguration();
        var connectionString = configuration.GetConnectionString("Postgres");

        var optionsBuilder = new DbContextOptionsBuilder<QuizardDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new QuizardDbContext(optionsBuilder.Options);
    }
}