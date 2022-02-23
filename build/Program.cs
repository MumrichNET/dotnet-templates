using Cake.Common;
using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.NuGet.Push;
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
  public string BuildOutputPath { get; set; }
  public string MsBuildConriguration { get; set; }
  public string NugetOrgApiKey { get; set; }
  public string NugetOrgSource { get; set; }

  public BuildContext(ICakeContext context)
    : base(context)
  {
    MsBuildConriguration = context.Argument(nameof(MsBuildConriguration), "Release");
    BuildOutputPath = context.Argument(nameof(BuildOutputPath), "../build-output");
    NugetOrgApiKey = context.Argument<string>(nameof(NugetOrgApiKey), null);
    NugetOrgSource = context.Argument(nameof(NugetOrgSource), "https://api.nuget.org/v3/index.json");
  }
}

public abstract class PackTaskBase : FrostingTask<BuildContext>
{
  protected static void PackCsproj(BuildContext context, string csprojName)
  {
    context.DotNetPack($"../{csprojName}/{csprojName}.csproj", new DotNetPackSettings
    {
      Configuration = context.MsBuildConriguration,
      NoLogo = true,
      IncludeSymbols = true,
      OutputDirectory = context.BuildOutputPath
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

[TaskName("NugetPublish")]
[IsDependentOn(typeof(PackSpaDevMiddlewareTask))]
[IsDependentOn(typeof(PackAkkaExtTask))]
public class NugetPublishTask : FrostingTask<BuildContext>
{
  public override void Run(BuildContext context)
  {
    if (string.IsNullOrWhiteSpace(context.NugetOrgApiKey))
    {
      throw new CakeException($"Property '{nameof(context.NugetOrgApiKey)}' must be set!");
    }

    foreach (var nugetFile in context.GetFiles($"{context.BuildOutputPath}/*.nupkg"))
    {
      context.DotNetNuGetPush(nugetFile, new DotNetNuGetPushSettings
      {
        ApiKey = context.NugetOrgApiKey,
        Source = context.NugetOrgSource,
        SkipDuplicate = true
      });
    }
  }
}
