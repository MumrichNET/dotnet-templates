using System.IO;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

using SpaDevServer.HostedServices;

using web_spa_vue;
using web_spa_vue.Extensions;

var builder = WebApplication.CreateBuilder(args);
var appSettings = builder.Configuration.Get<AppSettings>();

builder.Services.AddSingleton(appSettings);
builder.Services.AddControllersWithViews();

builder.RegisterSinglePageAppMiddleware(appSettings.SinglePageApps);

var app = builder.Build();

app.UseHttpsRedirection();

#if RELEASE
var clientAppRoot = Path.Combine(builder.Environment.ContentRootPath, "client-app/dist");
app.UseStaticFiles(new StaticFileOptions
{
  FileProvider = new PhysicalFileProvider(clientAppRoot),
  RequestPath = ""
});
#endif

app.UseRouting();
app.MapReverseProxy();

app.MapControllers();
app.MapDefaultControllerRoute();

#if RELEASE
var clientAppIndex = Path.Combine(clientAppRoot, "index.html");
app.MapGet("/", async context => await context.Response.SendFileAsync(clientAppIndex));
#endif

app.Run();
