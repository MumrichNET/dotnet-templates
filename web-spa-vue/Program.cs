using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SpaDevServer.HostedServices;

using web_spa_vue;

var builder = WebApplication.CreateBuilder(args);
var appSettings = builder.Configuration.Get<AppSettings>();

builder.Services.AddSingleton(appSettings);

#if DEBUG
builder.Services.AddHostedService<ViteJsDevelopmentService>();
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
#endif

var app = builder.Build();

app.UseRouting();
#if DEBUG
app.MapReverseProxy();
#endif

app.Run();
