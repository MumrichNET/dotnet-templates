using System;
using System.IO;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Mumrich.AkkaHost;
using Mumrich.AkkaHost.HostedServices;
using Mumrich.SpaDevMiddleware.Contracts;
using Mumrich.SpaDevMiddleware.HostedServices;

Console.WriteLine("Hello, World!");

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
      var hostSettings = hostContext.Configuration.Get<HostSettings>();
      services.AddSingleton(hostSettings);
      services.AddSingleton<ISpaDevServerSettings>(hostSettings);
      services.AddHostedService<ParentObserverService>();
      services.AddHostedService<SpaDevelopmentService>();
    })
    .Build();

host.Run();