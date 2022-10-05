using System;
using System.Threading;

using DotNetify;

namespace WebSpaVue.ClientViewModels
{
  public class HelloDotnetify : BaseVM
  {
    private readonly Timer _timer;

    public HelloDotnetify()
    {
      _timer = new Timer(_ =>
      {
        Changed(nameof(ServerTime));
        PushUpdates();
      }, null, 0, 1000);
    }

    public string Greetings => "Hello Dotnetify!";
    public DateTime ServerTime => DateTime.Now;

    public override void Dispose()
    {
      _timer.Dispose();
      GC.SuppressFinalize(this);
    }
  }
}