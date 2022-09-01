using Mumrich.SpaDevMiddleware;
using Mumrich.SpaDevMiddleware.Contracts;

public class AppSettings : ISpaDevServerSettings
{
    public Dictionary<string, SpaSettings> SinglePageApps { get; set; } = new();
}