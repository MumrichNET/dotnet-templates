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

public abstract class PackTaskBase : FrostingTask<BuildContext>
{
  protected static void PackCsproj(BuildContext context, string csprojName)
  {
    context.DotNetPack($"../{csprojName}/{csprojName}.csproj", new DotNetPackSettings
    {
      Configuration = "Release",
      NoLogo = true,
      IncludeSymbols = true,
      OutputDirectory = "../build-output"
    });
  }
}

public class PackSpaDevMiddlewareTask : PackTaskBase
{
  public override void Run(BuildContext context)
  {
    const string csprojName = "Mumrich.SpaDevMiddleware";

    PackCsproj(context, csprojName);
  }
}

public class PackAkkaExtTask : PackTaskBase
{
  public override void Run(BuildContext context)
  {
    const string csprojName = "Mumrich.AkkaExt";

    PackCsproj(context, csprojName);
  }
}

[TaskName("Default")]
[IsDependentOn(typeof(PackSpaDevMiddlewareTask))]
[IsDependentOn(typeof(PackAkkaExtTask))]
public class DefaultTask : FrostingTask<BuildContext>
{
}
