using DotNetify;

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
builder.Services.AddSwaggerDocument();
builder.Services.AddControllersWithViews();
builder.Services.AddCors();
builder.Services.AddMemoryCache();
builder.Services.AddSignalR();
builder.Services.AddDotNetify();

builder.RegisterSinglePageAppMiddleware(appSettings.SinglePageApps);

var app = builder.Build();

app.UseCors(corsPolicyBuilder => corsPolicyBuilder
  .AllowAnyMethod()
  .AllowAnyHeader()
  .WithOrigins("http://localhost:8080")
  .AllowCredentials());

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRouting();
app.UseOpenApi();
app.UseSwaggerUi3();
app.MapControllers();
app.MapDefaultControllerRoute();
app.UseWebSockets();
app.UseDotNetify();
app.MapHub<DotNetifyHub>("/dotnetify");
app.MapSinglePageApps(appSettings.SinglePageApps);

app.Run();
