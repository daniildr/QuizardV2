using Asp.Versioning;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Quizard.Application.GameEngine.Extensions;
using Quizard.Application.LicensingService.Extensions;
using Quizard.Application.ScenarioService.Extensions;
using Quizard.Infrastructure.CacheManager.Extensions;
using Quizard.Infrastructure.DataProvider.Extensions;
using Quizard.SignalR;
using Quizard.WebApi.Extensions;
using Quizard.WebApi.Options;
using Quizard.WebApi.Routes;

namespace Quizard.WebApi;

internal sealed class Startup
{
    private readonly ApiAuthorOptions _apiAuthorOptions;
    private readonly IConfiguration _configuration;
    private readonly CancellationTokenSource _cancellationTokenSource;
    
    /// <summary> Конструктор стартапа </summary>
    /// <param name="configuration"> Конфигурация проекта </param>
    /// <param name="cancellationTokenSource"> Токен отмены </param>
    public Startup(IConfiguration configuration, CancellationTokenSource cancellationTokenSource)
    {
        _configuration = configuration;
        _cancellationTokenSource = cancellationTokenSource;
        _apiAuthorOptions = _configuration
            .GetSection(ApiAuthorOptions.ConfigurationSection)
            .Get<ApiAuthorOptions>()!;
    }

    /// <summary> Конфигурирует сервисы </summary>
    /// <param name="services"> Коллекция сервисов </param>
    public void ConfigureServices(IServiceCollection services)
    {
        var apiOptions = _configuration.GetSection(ApiOptions.ConfigurationSection).Get<ApiOptions>() 
                         ?? throw new NullReferenceException("Секция ApiOptions не указана в appsettings.json");
        services.AddSingleton<IOptions<ApiOptions>>(new OptionsWrapper<ApiOptions>(apiOptions));
        
        services.AddDataProviderServices(_configuration);
        services.AddLicensingService(_configuration);
        services.AddScenarioService(_configuration);
        services.AddCacheManager(_configuration);
        services
            .AddSignalR(options =>
            {
                // по умолчанию было 30 сек — увеличим до 120:
                options.ClientTimeoutInterval = TimeSpan.FromSeconds(120);
            })
            .AddNewtonsoftJsonProtocol(opts =>
            {
                opts.PayloadSerializerSettings.ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                };
                opts.PayloadSerializerSettings.NullValueHandling    = NullValueHandling.Ignore;
                opts.PayloadSerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
            });
        services.AddGameEngine(_configuration);
        
        services.AddSingleton(_cancellationTokenSource);
        services.AddTunedRateLimiter();
        services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicyNames.PublicPolicy, builder =>
            {
                builder
                    .SetIsOriginAllowed(_ => true)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
        
        services
            .AddControllers()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy
                    {
                        ProcessDictionaryKeys = true,
                        OverrideSpecifiedNames = false
                    }
                };
                options.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
                options.SerializerSettings.NullValueHandling    = NullValueHandling.Ignore;
            });
        services
            .AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(
                    GetType().Assembly.GetName().Version!.Major, 
                    GetType().Assembly.GetName().Version!.Minor);
                options.ReportApiVersions = true;
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
        services.AddEndpointsApiExplorer();
        
        services.AddTunedSwagger(_apiAuthorOptions, GetType().Assembly.GetName().Version!.Major);
    }
    
    /// <summary> Конфигурирует web приложение </summary>
    /// <param name="app"> Приложение </param>
    public static void Configure(IApplicationBuilder app)
    {   
        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles(new StaticFileOptions
        {
            OnPrepareResponse = ctx =>
            {
                ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
                ctx.Context.Response.Headers.Append("Cache-Control", "public, max-age=2592000");
            }
        });
        app.UseRouting();
        app.UseCors();
        app.UseRateLimiter();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapFallbackToFile("index.html");
            endpoints.MapHub<GameHub>("/hubs/game").RequireCors(CorsPolicyNames.PublicPolicy);
        });
        app.UseQuizardApiSwagger();
    }
}