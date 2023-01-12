using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

using Akka.Actor;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Mumrich.HelpersAndExtensions;
using Mumrich.SpaDevMiddleware.Domain.Contracts;
using Mumrich.SpaDevMiddleware.Domain.Models;
using Mumrich.SpaDevMiddleware.Extensions;
using Mumrich.SpaDevMiddleware.Utils;

using Newtonsoft.Json;

namespace Mumrich.SpaDevMiddleware.Actors
{
  public record StartProcessCommand;

  // ReSharper disable once ClassNeverInstantiated.Global
  public class ProcessRunnerActor : ReceiveActor
  {
    private const string DefaultRegex = "running at";
    private static readonly Regex AnsiColorRegex = new("\x001b\\[[0-9;]*m", RegexOptions.None, TimeSpan.FromSeconds(1));
    private static readonly TimeSpan RegexMatchTimeout = TimeSpan.FromMinutes(5);
    private readonly ILogger<ProcessRunnerActor> _logger;

    public ProcessRunnerActor(IServiceProvider serviceProvider, SpaSettings spaSettings)
    {
      var serviceProviderScope = serviceProvider.CreateScope();
      var spaDevServerSettings = serviceProviderScope.ServiceProvider.GetService<ISpaDevServerSettings>();
      _logger = serviceProviderScope.ServiceProvider.GetService<ILogger<ProcessRunnerActor>>();

      var regex = spaSettings.Regex;

      _logger.LogInformation("SpaSettings: {}", JsonConvert.SerializeObject(spaSettings, Formatting.Indented));

      var processStartInfo = spaSettings.GetProcessStartInfo(spaDevServerSettings);

      _logger.LogInformation("{}: {} {} (cwd: '{}')", nameof(processStartInfo), processStartInfo.FileName, processStartInfo.Arguments, processStartInfo.WorkingDirectory);

      ReceiveAsync<StartProcessCommand>(async _ =>
      {
        RunnerProcess = LaunchNodeProcess(processStartInfo);

        StdOut = new EventedStreamReader(RunnerProcess.StandardOutput);
        StdErr = new EventedStreamReader(RunnerProcess.StandardError);

        AttachToLogger();

        using var stdErrReader = new EventedStreamStringReader(StdErr);

        try
        {
          // Although the Vue dev server may eventually tell us the URL it's listening on,
          // it doesn't do so until it's finished compiling, and even then only if there were
          // no compiler warnings. So instead of waiting for that, consider it ready as soon
          // as it starts listening for requests.
          await StdOut.WaitForMatch(new Regex(
            !string.IsNullOrWhiteSpace(regex) ? regex : DefaultRegex,
            RegexOptions.None,
            RegexMatchTimeout));
        }
        catch (EndOfStreamException ex)
        {
          throw new InvalidOperationException(
            $"The Command '{spaSettings.NodeStartScript}' exited without indicating that the " +
            "server was listening for requests. The error output was: " +
            $"{stdErrReader.ReadAsString()}", ex);
        }
      });

      Self.Tell(new StartProcessCommand());
    }

    private Process RunnerProcess { get; set; }

    private EventedStreamReader StdErr { get; set; }

    private EventedStreamReader StdOut { get; set; }

    protected override void PostStop()
    {
      RunnerProcess.KillProcessTree();
    }

    private static Process LaunchNodeProcess(ProcessStartInfo startInfo)
    {
      try
      {
        var process = Process.Start(startInfo);

        if (process != null)
        {
          process.EnableRaisingEvents = true;
        }

        return process;
      }
      catch (Exception ex)
      {
        var message = $"Failed to start '{startInfo.FileName}'. To resolve this:.\n\n"
                      + $"[1] Ensure that '{startInfo.FileName}' is installed and can be found in one of the PATH directories.\n"
                      + $"    Current PATH enviroment variable is: {Environment.GetEnvironmentVariable("PATH")}\n"
                      + "    Make sure the executable is in one of those directories, or update your PATH.\n\n"
                      + "[2] See the InnerException for further details of the cause.";
        throw new InvalidOperationException(message, ex);
      }
    }

    private static string StripAnsiColors(string line) => AnsiColorRegex.Replace(line, string.Empty);

    private void AttachToLogger()
    {
      void StdErrOnReceivedLine(string line)
      {
        if (string.IsNullOrWhiteSpace(line))
        {
          return;
        }

        // NPM tasks commonly emit ANSI colors, but it wouldn't make sense to forward
        // those to loggers (because a logger isn't necessarily any kind of terminal)
        // making this console for debug purpose
        if (line.StartsWith("<s>"))
        {
          line = line[3..];
        }

        if (_logger == null)
        {
         Console.Error.WriteLine(line);
        }
        else
        {
          var effectiveLine = StripAnsiColors(line).TrimEnd('\n');
          _logger.LogError("{}", effectiveLine);
        }
      }

      void StdOutOnReceivedLine(string line)
      {
        if (_logger == null)
        {
         Console.WriteLine(line);
        }
        else
        {
          var effectiveLine = StripAnsiColors(line).TrimEnd('\n');
          _logger.LogInformation("{}", effectiveLine);
        }
      }

      // When the NPM task emits complete lines, pass them through to the real logger
      StdOut.OnReceivedLine += StdOutOnReceivedLine;
      StdErr.OnReceivedLine += StdErrOnReceivedLine;

      // But when it emits incomplete lines, assume this is progress information and
      // hence just pass it through to StdOut regardless of logger config.
      StdErr.OnReceivedChunk += chunk =>
      {
        if (chunk.Array == null)
        {
          return;
        }

        var containsNewline = Array.IndexOf(chunk.Array, '\n', chunk.Offset, chunk.Count) >= 0;

        if (!containsNewline)
        {
          _logger.LogInformation("{}", new string(chunk.Array));
        }
      };
    }
  }
}