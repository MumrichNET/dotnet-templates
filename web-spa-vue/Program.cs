using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SpaDevServer.Contracts;

using web_spa_vue;
using web_spa_vue.Extensions;

var builder = WebApplication.CreateBuilder(args);
var appSettings = builder.Configuration.Get<AppSettings>();

builder.Services.AddSingleton(appSettings);
builder.Services.AddSingleton<ISpaDevServerSettings, AppSettings>();
builder.Services.AddControllersWithViews();

builder.RegisterSinglePageAppMiddleware(appSettings.SinglePageApps);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();
app.MapDefaultControllerRoute();
app.MapSinglePageApps(appSettings.SinglePageApps);

app.Run();
