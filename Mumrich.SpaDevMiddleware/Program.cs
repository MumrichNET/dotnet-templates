using System.IO;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Mumrich.SpaDevMiddleware.Domain;
using Mumrich.SpaDevMiddleware.Domain.Contracts;
using Mumrich.SpaDevMiddleware.HostedServices;

IHost host = Host.CreateDefaultBuilder(args)
  .ConfigureHostConfiguration(configHost =>
  {
    configHost.SetBasePath(Directory.GetCurrentDirectory());
    configHost.AddJsonFile("appsettings.json");
    configHost.AddEnvironmentVariables(prefix: "HOST_");
    configHost.AddCommandLine(args);
  })
  .ConfigureServices((hostContext, services) =>
  {
    var hostSettings = hostContext.Configuration.Get<DefaultAppSettings>();
    services.AddSingleton(hostSettings);
    services.AddSingleton<ISpaDevServerSettings>(hostSettings);
    services.AddHostedService<WindowsParentProcessObserverService>();
    services.AddHostedService<SpaDevelopmentService>();
  })
  .Build();

host.Run();