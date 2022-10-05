using System.Collections.Generic;
using System.IO;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Mumrich.SpaDevMiddleware.Contracts;
using Mumrich.SpaDevMiddleware.Extensions;
using Mumrich.SpaDevMiddleware.Models;

namespace Template.WebSpaVue3Empty;

public class AppSettings : ISpaDevServerSettings
{
  public Dictionary<string, SpaSettings> SinglePageApps { get; set; } = new();
  public string SpaRootPath { get; set; } = Directory.GetCurrentDirectory();
}

public static class Program
{
  public static void Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);
    var appSettings = builder.Configuration.Get<AppSettings>();

    builder.Services.AddControllersWithViews();
    builder.Services.AddCors();
    builder.RegisterSinglePageAppDevMiddleware(appSettings);

    var app = builder.Build();

    app.UseStaticFiles();
    app.UseHttpsRedirection();
    app.UseRouting();
    app.UseCors();
    app.MapControllers();
    app.MapSinglePageApps(appSettings);

    app.Run();
  }
}
