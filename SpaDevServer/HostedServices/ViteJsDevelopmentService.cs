using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SpaDevServer.HostedServices
{
  public class ViteJsDevelopmentService : IHostedService
  {
    internal const string DefaultRegex = "running at";
    private static Regex AnsiColorRegex = new("\x001b\\[[0-9;]*m", RegexOptions.None, TimeSpan.FromSeconds(1));
    private static TimeSpan RegexMatchTimeout = TimeSpan.FromMinutes(5);
    private readonly ILogger<ViteJsDevelopmentService> _logger;
    // This is a development-time only feature, so a very long timeout is fine

    public ViteJsDevelopmentService(ILogger<ViteJsDevelopmentService> logger)
    {
      _logger = logger;
    }

    internal Process RunnerProcess { get; set; }

    internal EventedStreamReader StdErr { get; set; }

    internal EventedStreamReader StdOut { get; set; }

    public void AttachToLogger()
    {
      // When the NPM task emits complete lines, pass them through to the real logger
      StdOut.OnReceivedLine += line =>
      {
        if (!string.IsNullOrWhiteSpace(line))
        {
          // NPM tasks commonly emit ANSI colors, but it wouldn't make sense to forward
          // those to loggers (because a logger isn't necessarily any kind of terminal)
          // making this console for debug purpose
          if (line.StartsWith("<s>"))
          {
            line = line.Substring(3);
          }

          if (_logger == null)
          {
            Console.Error.WriteLine(line);
          }
          else
          {
            _logger.LogInformation(StripAnsiColors(line).TrimEnd('\n'));
          }
        }
      };

      StdErr.OnReceivedLine += line =>
      {
        if (!string.IsNullOrWhiteSpace(line))
        {
          // making this console for debug purpose
          if (line.StartsWith("<s>"))
          {
            line = line.Substring(3);
          }

          if (_logger == null)
          {
            Console.Error.WriteLine(line);
          }
          else
          {
            _logger.LogError(StripAnsiColors(line).TrimEnd('\n'));
          }
        }
      };

      // But when it emits incomplete lines, assume this is progress information and
      // hence just pass it through to StdOut regardless of logger config.
      StdErr.OnReceivedChunk += chunk =>
      {
        var containsNewline = Array.IndexOf(chunk.Array, '\n', chunk.Offset, chunk.Count) >= 0;

        if (!containsNewline)
        {
          Console.Write(chunk.Array, chunk.Offset, chunk.Count);
        }
      };
    }

    public void Kill()
    {
      try { RunnerProcess?.Kill(); } catch { }
      try { RunnerProcess?.WaitForExit(); } catch { }
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
      var regex = "dev server running at:";
      var npmScriptName = "/c yarn dev";
      var processStartInfo = new ProcessStartInfo("cmd")
      {
        Arguments = npmScriptName,
        UseShellExecute = false,
        RedirectStandardInput = true,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        WorkingDirectory = Path.Combine(Directory.GetCurrentDirectory(), "client-app")
      };

      processStartInfo.Environment["HMR_PORT"] = Environment.GetEnvironmentVariable("ASPNETCORE_HTTPS_PORT") ?? "7189";
      processStartInfo.Environment["HMR_PROTOCOL"] = "wss";

      RunnerProcess = LaunchNodeProcess(processStartInfo);

      StdOut = new EventedStreamReader(RunnerProcess.StandardOutput);
      StdErr = new EventedStreamReader(RunnerProcess.StandardError);

      AppDomain.CurrentDomain.DomainUnload += (s, e) => Kill();
      AppDomain.CurrentDomain.ProcessExit += (s, e) => Kill();
      AppDomain.CurrentDomain.UnhandledException += (s, e) => Kill();

      AttachToLogger();

      using var stdErrReader = new EventedStreamStringReader(StdErr);

      try
      {
        // Although the Vue dev server may eventually tell us the URL it's listening on,
        // it doesn't do so until it's finished compiling, and even then only if there were
        // no compiler warnings. So instead of waiting for that, consider it ready as soon
        // as it starts listening for requests.
        await StdOut.WaitForMatch(new Regex(!string.IsNullOrWhiteSpace(regex) ? regex : DefaultRegex, RegexOptions.None, RegexMatchTimeout));
      }
      catch (EndOfStreamException ex)
      {
        throw new InvalidOperationException(
          $"The NPM script '{npmScriptName}' exited without indicating that the " +
          $"server was listening for requests. The error output was: " +
          $"{stdErrReader.ReadAsString()}", ex);
      }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      Kill();

      return Task.CompletedTask;
    }

    private static Process LaunchNodeProcess(ProcessStartInfo startInfo)
    {
      try
      {
        var process = Process.Start(startInfo);

        // See equivalent comment in OutOfProcessNodeInstance.cs for why
        process.EnableRaisingEvents = true;

        return process;
      }
      catch (Exception ex)
      {
        var message = $"Failed to start '{startInfo.FileName}'. To resolve this:.\n\n"
                      + $"[1] Ensure that '{startInfo.FileName}' is installed and can be found in one of the PATH directories.\n"
                      + $"    Current PATH enviroment variable is: { Environment.GetEnvironmentVariable("PATH") }\n"
                      + "    Make sure the executable is in one of those directories, or update your PATH.\n\n"
                      + "[2] See the InnerException for further details of the cause.";
        throw new InvalidOperationException(message, ex);
      }
    }

    private static string StripAnsiColors(string line)
          => AnsiColorRegex.Replace(line, string.Empty);
  }

  internal class EventedStreamReader
  {
    private readonly StringBuilder _linesBuffer;

    private readonly StreamReader _streamReader;

    public EventedStreamReader(StreamReader streamReader)
    {
      _streamReader = streamReader ?? throw new ArgumentNullException(nameof(streamReader));
      _linesBuffer = new StringBuilder();
      Task.Factory.StartNew(Run);
    }

    public delegate void OnReceivedChunkHandler(ArraySegment<char> chunk);

    public delegate void OnReceivedLineHandler(string line);

    public delegate void OnStreamClosedHandler();

    public event OnReceivedChunkHandler OnReceivedChunk;

    public event OnReceivedLineHandler OnReceivedLine;

    public event OnStreamClosedHandler OnStreamClosed;

    public Task<Match> WaitForMatch(Regex regex)
    {
      var tcs = new TaskCompletionSource<Match>();
      var completionLock = new object();

      OnReceivedLineHandler onReceivedLineHandler = null;
      OnStreamClosedHandler onStreamClosedHandler = null;

      void ResolveIfStillPending(Action applyResolution)
      {
        lock (completionLock)
        {
          if (!tcs.Task.IsCompleted)
          {
            OnReceivedLine -= onReceivedLineHandler;
            OnStreamClosed -= onStreamClosedHandler;
            applyResolution();
          }
        }
      }

      onReceivedLineHandler = line =>
      {
        var match = regex.Match(line);
        if (match.Success)
        {
          ResolveIfStillPending(() => tcs.SetResult(match));
        }
      };

      onStreamClosedHandler = () =>
      {
        ResolveIfStillPending(() => tcs.SetException(new EndOfStreamException()));
      };

      OnReceivedLine += onReceivedLineHandler;
      OnStreamClosed += onStreamClosedHandler;

      return tcs.Task;
    }

    private void OnChunk(ArraySegment<char> chunk)
    {
      var dlg = OnReceivedChunk;
      dlg?.Invoke(chunk);
    }

    private void OnClosed()
    {
      var dlg = OnStreamClosed;
      dlg?.Invoke();
    }

    private void OnCompleteLine(string line)
    {
      var dlg = OnReceivedLine;
      dlg?.Invoke(line);
    }

    private async Task Run()
    {
      var buf = new char[8 * 1024];
      while (true)
      {
        var chunkLength = await _streamReader.ReadAsync(buf, 0, buf.Length);
        if (chunkLength == 0)
        {
          OnClosed();
          break;
        }

        OnChunk(new ArraySegment<char>(buf, 0, chunkLength));

        int lineBreakPos = -1;
        int startPos = 0;

        // get all the newlines
        while ((lineBreakPos = Array.IndexOf(buf, '\n', startPos, chunkLength - startPos)) >= 0 && startPos < chunkLength)
        {
          var length = lineBreakPos - startPos;
          _linesBuffer.Append(buf, startPos, length);
          OnCompleteLine(_linesBuffer.ToString());
          _linesBuffer.Clear();
          startPos = lineBreakPos + 1;
        }

        // get the rest
        if (lineBreakPos < 0 && startPos < chunkLength)
        {
          _linesBuffer.Append(buf, startPos, chunkLength - startPos);
        }
      }
    }
  }

  internal class EventedStreamStringReader : IDisposable
  {
    private EventedStreamReader _eventedStreamReader;
    private bool _isDisposed;
    private StringBuilder _stringBuilder = new StringBuilder();

    public EventedStreamStringReader(EventedStreamReader eventedStreamReader)
    {
      _eventedStreamReader = eventedStreamReader
                             ?? throw new ArgumentNullException(nameof(eventedStreamReader));
      _eventedStreamReader.OnReceivedLine += OnReceivedLine;
    }

    public void Dispose()
    {
      if (!_isDisposed)
      {
        _eventedStreamReader.OnReceivedLine -= OnReceivedLine;
        _isDisposed = true;
      }
    }

    public string ReadAsString() => _stringBuilder.ToString();

    private void OnReceivedLine(string line) => _stringBuilder.AppendLine(line);
  }
}
