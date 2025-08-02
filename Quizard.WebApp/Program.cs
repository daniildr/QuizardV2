using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Quizard.WebApp;
using Quizard.WebApp.Options;
using Quizard.WebApp.Services;
using Serilog;
using Serilog.Events;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .MinimumLevel.Override("Microsoft.AspNetCore.SignalR", LogEventLevel.Verbose)
    .MinimumLevel.Override("Microsoft.AspNetCore.Http.Connections", LogEventLevel.Verbose)
    .WriteTo.BrowserConsole()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(Log.Logger);

builder.Services.Configure<RackIdOptions>(builder.Configuration.GetSection("RackIds"));
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<ScenarioService>();
builder.Services.AddScoped<HeaderService>();
builder.Services.AddScoped<GameService>();

await builder.Build().RunAsync();