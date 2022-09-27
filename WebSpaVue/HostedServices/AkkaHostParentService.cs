using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Mumrich.AkkaHost.HostedServices;
using Mumrich.SpaDevMiddleware.Utils;

namespace WebSpaVue.HostedServices
{
  public class AkkaHostParentService : IHostedService
  {
    private readonly ILogger<ExampleHostedService> _logger;
    private Process _akkaHostProcess;
    private EventedStreamReader _stdOut;
    private EventedStreamReader _stdErr;

    public AkkaHostParentService(ILogger<ExampleHostedService> logger, IHostApplicationLifetime appLifetime)
    {
      _logger = logger;
    }
    public Task StartAsync(CancellationToken cancellationToken)
    {
      var processStartInfo = new ProcessStartInfo("dotnet")
      {
        Arguments = "Mumrich.AkkaHost.dll",
        WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
        UseShellExecute = false,
        RedirectStandardInput = true,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
      };
      _akkaHostProcess = Process.Start(processStartInfo);
      _akkaHostProcess.EnableRaisingEvents = true;

      _stdOut = new EventedStreamReader(_akkaHostProcess.StandardOutput);
      _stdErr = new EventedStreamReader(_akkaHostProcess.StandardError);

      _stdOut.OnReceivedLine += (string line) =>
      {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(line);
      };

      _stdErr.OnReceivedLine += (string line) =>
      {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(line);
      };

      return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
      _akkaHostProcess.Kill(entireProcessTree: true);
      await _akkaHostProcess.WaitForExitAsync();
    }
  }
}