using Quizard.Core.Extensions;
using Quizard.Core.Interfaces;

namespace Quizard.WebApi;

internal static class Program
{
    private static readonly CancellationTokenSource CancellationTokenSource = new();
    private static IConfigurationRoot Configuration { get; set; } = null!;
    
    internal static async Task Main(string[] args)
    {
        Configuration = Core.Utils.ConfigurationBuilder.BuildConfiguration();
        var cancellationTokenSource = new CancellationTokenSource();
        var host = CreateHostBuilder(args).Build();
        
        using (var scope = host.Services.CreateScope())
        {
            var initializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
            await initializer.InitializeAsync(CancellationTokenSource.Token);
        }
        
        await host.RunAsync(token: cancellationTokenSource.Token);
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
        => Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, builder) =>
            {
                builder.SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables();
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureKestrel(serverOptions =>
                {
                    serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(10);
                    serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(15);    
                    serverOptions.Limits.MaxRequestBodySize = 1024 * 1024 * 300;         
                });
                webBuilder.UseStartup<Startup>(_ => new Startup(Configuration, CancellationTokenSource));
            })
            .ConfigureLogging(logging =>
            {
                logging.ConfigureStructuredLogger(Configuration);
            });
}