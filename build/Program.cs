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
  public BuildContext(ICakeContext context)
    : base(context)
  {
    MsBuildConriguration = context.Argument(nameof(MsBuildConriguration), "Release");
    BuildOutputPath = context.Argument(nameof(BuildOutputPath), "../build-output");
    NugetOrgApiKey = context.Argument<string>(nameof(NugetOrgApiKey), null);
    NugetOrgSource = context.Argument(nameof(NugetOrgSource), "https://api.nuget.org/v3/index.json");
  }

  public string BuildOutputPath { get; set; }
  public string MsBuildConriguration { get; set; }
  public string NugetOrgApiKey { get; set; }
  public string NugetOrgSource { get; set; }
}

[TaskName("Default")]
[IsDependentOn(typeof(PackMumrichSpaDevMiddlewareTask))]
[IsDependentOn(typeof(PackMumrichAkkaExtTask))]
[IsDependentOn(typeof(PackMumrichExtensionsTask))]
public class DefaultTask : FrostingTask<BuildContext>
{
}

public class PackMumrichAkkaExtTask : PackTaskBase
{
  public override void Run(BuildContext context)
  {
    const string csprojName = "Mumrich.AkkaExt";

    PackCsproj(context, csprojName);
  }
}

public class PackMumrichExtensionsTask : PackTaskBase
{
  public override void Run(BuildContext context)
  {
    const string csprojName = "Mumrich.HelpersAndExtensions";

    PackCsproj(context, csprojName);
  }
}

public class PackMumrichSpaDevMiddlewareTask : PackTaskBase
{
  public override void Run(BuildContext context)
  {
    const string csprojName = "Mumrich.SpaDevMiddleware";

    PackCsproj(context, csprojName);
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
      IncludeSource = true,
      OutputDirectory = context.BuildOutputPath
    });
  }
}

[TaskName("NugetPublish")]
[IsDependentOn(typeof(DefaultTask))]
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