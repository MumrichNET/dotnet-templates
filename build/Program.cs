using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Pack;
using Cake.Core;
using Cake.Frosting;

namespace Build;

public static class Program
{
  public static int Main(string[] args)
  {
    return new CakeHost()
      .UseContext<BuildContext>()
      .Run(args);
  }
}

public class BuildContext : FrostingContext
{
  public bool Delay { get; set; }

  public BuildContext(ICakeContext context)
    : base(context)
  {
    Delay = context.Arguments.HasArgument("delay");
  }
}

[TaskName("Default")]
public class DefaultTask : FrostingTask<BuildContext>
{
  public override void Run(BuildContext context)
  {
    context.DotNetPack("../Mumrich.SpaDevMiddleware/Mumrich.SpaDevMiddleware.csproj", new DotNetPackSettings
    {
      Configuration = "Release",
      NoLogo = true,
      IncludeSymbols = true,
      OutputDirectory = "../build-output"
    });
  }
}
