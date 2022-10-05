using System;
using System.Text;

namespace Mumrich.SpaDevMiddleware.Utils;

internal class EventedStreamStringReader : IDisposable
{
  private readonly EventedStreamReader _eventedStreamReader;
  private readonly StringBuilder _stringBuilder = new();
  private bool _isDisposed;

  public EventedStreamStringReader(EventedStreamReader eventedStreamReader)
  {
    _eventedStreamReader = eventedStreamReader ?? throw new ArgumentNullException(nameof(eventedStreamReader));
    _eventedStreamReader.OnReceivedLine += OnReceivedLine;
  }

  public void Dispose()
  {
    if (_isDisposed)
    {
      return;
    }

    _eventedStreamReader.OnReceivedLine -= OnReceivedLine;
    _isDisposed = true;
  }

  public string ReadAsString() => _stringBuilder.ToString();

  private void OnReceivedLine(string line) => _stringBuilder.AppendLine(line);
}