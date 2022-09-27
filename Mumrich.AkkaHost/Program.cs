using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Mumrich.AkkaHost.HostedServices;
using Mumrich.SpaDevMiddleware.HostedServices;

Console.WriteLine("Hello, World!");

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
      services.AddHostedService<ExampleHostedService>();
      //services.AddHostedService<SpaDevelopmentService>();
    })
    .Build();

host.Run();