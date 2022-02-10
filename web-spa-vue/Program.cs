using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using web_spa_vue.HostedServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<SpaDevelopmentService>();
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseRouting();
app.MapReverseProxy();

app.Run();
