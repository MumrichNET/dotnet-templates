using System.Diagnostics;

using DotNetify;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Mumrich.SpaDevMiddleware.Domain.Contracts;
using Mumrich.SpaDevMiddleware.Extensions;

using WebSpaVue;

var p = Process.GetCurrentProcess();

var builder = WebApplication.CreateBuilder(args);
var appSettings = builder.Configuration.Get<AppSettings>();

// AppSettings & variants
builder.Services.AddSingleton(appSettings);
builder.Services.AddSingleton<ISpaDevServerSettings>(appSettings);

builder.Services.AddSwaggerDocument();
builder.Services.AddControllersWithViews();
builder.Services.AddCors();
builder.Services.AddMemoryCache();
builder.Services.AddSignalR();
builder.Services.AddDotNetify();

builder.RegisterSinglePageAppDevMiddleware(appSettings);

var app = builder.Build();

app.UseCors(corsPolicyBuilder => corsPolicyBuilder
  .AllowAnyMethod()
  .AllowAnyHeader()
  .WithOrigins("http://localhost:8901")
  .AllowCredentials());

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors();
app.UseOpenApi();
app.UseSwaggerUi3();
app.MapControllers();
app.MapDefaultControllerRoute();
app.UseWebSockets();
app.UseDotNetify();
app.MapHub<DotNetifyHub>("/dotnetify");

app.MapSinglePageApps(appSettings);

app.Run();