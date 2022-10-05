using System.IO;

using Akka.Actor;

using Mumrich.AkkaExt.Extensions;
using Mumrich.SpaDevMiddleware.Contracts;
using Mumrich.SpaDevMiddleware.Models;

namespace Mumrich.SpaDevMiddleware.Actors
{
  public class SpaBuilderActor : ReceiveActor
  {
    public SpaBuilderActor(ISpaDevServerSettings spaDevServerSettings)
    {
      foreach ((string _, SpaSettings value) in spaDevServerSettings.SinglePageApps)
      {
        DirectoryInfo directoryInfo = new(value.SpaRootPath);
        Context.ActorOfWithNameAndArgs<ProcessRunnerActor>(directoryInfo.Name, value);
      }
    }
  }
}