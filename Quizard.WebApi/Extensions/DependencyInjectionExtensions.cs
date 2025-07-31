using System.Reflection;
using System.Threading.RateLimiting;
using Microsoft.OpenApi.Models;
using Quizard.WebApi.Constants;
using Quizard.WebApi.Options;
using Quizard.WebApi.Routes;

namespace Quizard.WebApi.Extensions;

/// <summary> Расширения для введения DI зависимостей web api </summary>
internal static class DependencyInjectionExtensions
{
    /// <summary> Расширение для введения зависимостей сервиса авторизации и аутентификации </summary>
    /// <param name="services"> Коллекция сервисов </param>
    public static void AddTunedRateLimiter(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.AddPolicy(RateLimitLabels.PublicPolicy, context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    context.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 100,          
                        Window = TimeSpan.FromSeconds(1),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 10
                    }));
            
            options.AddPolicy(RateLimitLabels.LimitPolicy, context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    context.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 30,
                        Window = TimeSpan.FromSeconds(1),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 5
                    }));
            
            options.AddPolicy(RateLimitLabels.ExtraLimitPolicy, context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    context.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 1,
                        Window = TimeSpan.FromMinutes(5),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 3
                    }));
            
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = async (context, cancellationToken) =>
            {
                context.HttpContext.Response.ContentType = "application/json";
                await context.HttpContext.Response.WriteAsync(
                    "{\"Ошибка\": \"Превышен лимит запросов. Пожалуйста, повторите попытку позже.\"}",
                    cancellationToken);
            };
        });
    }
    
    /// <summary> Метод для внедрения зависимостей swagger-документации API контроллеров </summary>
    /// <param name="services"> Сервис провайдер </param>
    /// <param name="apiOptions"> Информация об авторе </param>
    /// <param name="majorVersion"> Версия API </param>
    public static void AddTunedSwagger(this IServiceCollection services, ApiAuthorOptions apiOptions, int majorVersion)
    {
        services.AddSwaggerGen(options =>
        {
            options.DescribeAllParametersInCamelCase();
            
            options.SwaggerDoc(ApiDomains.LicensingDomain, GetSwaggerInfoDoc(
                apiOptions, ApiDomains.LicensingDomain, "управления лицензией", majorVersion));
            
            options.SwaggerDoc(ApiDomains.GameDomain, GetSwaggerInfoDoc(
                apiOptions, ApiDomains.GameDomain, "управления игровой сессией", majorVersion));
            
            options.SwaggerDoc(ApiDomains.ScenariosDomain, GetSwaggerInfoDoc(
                apiOptions, ApiDomains.ScenariosDomain, "управления игровыми сценариями", majorVersion));
            
            options.SwaggerDoc(ApiDomains.StatisticsDomain, GetSwaggerInfoDoc(
                apiOptions, ApiDomains.StatisticsDomain, "работы с данными игровой статистики", majorVersion));
            
            options.SwaggerDoc(ApiDomains.HelpDeskDomain, GetSwaggerInfoDoc(
                apiOptions, ApiDomains.HelpDeskDomain, "справочного сервиса", majorVersion));
            
            options.IncludeXmlComments(
                Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Quizard.Core.xml"));
        });
    }

    /// <summary> Метод для включения документации swagger в сборку приложения </summary>
    /// <param name="app"> Инстанс приложения </param>
    public static void UseQuizardApiSwagger(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(uiOptions =>
        {
            var domainNames = typeof(ApiDomains)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi is { IsLiteral: true, IsInitOnly: false } && fi.FieldType == typeof(string))
                .Select(fi => (string)fi.GetRawConstantValue()!)
                .ToList();
            
            foreach (var domainName in domainNames)
            {
                uiOptions.SwaggerEndpoint(
                    $"/swagger/{domainName}/swagger.json",
                    domainName);
            }
            
            uiOptions.RoutePrefix = "swagger";
        });
    }
    
    private static OpenApiInfo GetSwaggerInfoDoc(
        ApiAuthorOptions apiOptions, string domain, string description, int majorVersion)
    {
        var contactInfo = new OpenApiContact
        {
            Name = apiOptions.Name,
            Email = apiOptions.Email
        };

        var licenseInfo = new OpenApiLicense { Name = apiOptions.License };
        
        return new OpenApiInfo
        {
            Title = string.Format(apiOptions.Title, domain), 
            Version = $"v{majorVersion}",
            Description = string.Format(apiOptions.Description, description), 
            Contact = contactInfo,
            License = licenseInfo
        };
    }
}