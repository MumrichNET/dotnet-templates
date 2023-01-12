using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;

using Mumrich.SpaDevMiddleware.Domain.Contracts;
using Mumrich.SpaDevMiddleware.Utils;

namespace Mumrich.SpaDevMiddleware.HostedServices
{
  public class AkkaHostParentService : IHostedService
  {
    private Process _akkaHostProcess;
    private EventedStreamReader _stdErr;
    private EventedStreamReader _stdOut;

    public Task StartAsync(CancellationToken cancellationToken)
    {
      var processStartInfo = new ProcessStartInfo("dotnet")
      {
        Arguments = $"{nameof(Mumrich)}.{nameof(SpaDevMiddleware)}.dll --{nameof(ISpaDevServerSettings.SpaRootPath)}={Directory.GetCurrentDirectory()}",
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

      _stdOut.OnReceivedLine += Console.WriteLine;

      _stdErr.OnReceivedLine += Console.WriteLine;

      return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
      _akkaHostProcess.Close();
      await _akkaHostProcess.WaitForExitAsync(cancellationToken);
    }
  }
}