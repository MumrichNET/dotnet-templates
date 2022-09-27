using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Mumrich.SpaDevMiddleware.Utils;

namespace Mumrich.SpaDevMiddleware.HostedServices
{
  public class AkkaHostParentService : IHostedService
  {
    private readonly ILogger<AkkaHostParentService> _logger;
    private Process _akkaHostProcess;
    private EventedStreamReader _stdOut;
    private EventedStreamReader _stdErr;

    public AkkaHostParentService(ILogger<AkkaHostParentService> logger, IHostApplicationLifetime appLifetime)
    {
      _logger = logger;
    }
    public Task StartAsync(CancellationToken cancellationToken)
    {
      var processStartInfo = new ProcessStartInfo("dotnet")
      {
        Arguments = $"Mumrich.AkkaHost.dll --SpaRootPath={Directory.GetCurrentDirectory()}",
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

      _stdOut.OnReceivedLine += (line) =>
      {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(line);
      };

      _stdErr.OnReceivedLine += (line) =>
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