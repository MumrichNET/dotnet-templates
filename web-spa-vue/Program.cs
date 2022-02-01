using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using web_spa_vue.HostedServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<SpaDevelopmentService>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
