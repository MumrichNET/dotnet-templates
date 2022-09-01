using Mumrich.SpaDevMiddleware.Extensions;

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
