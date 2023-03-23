using Build.Tasks;

using Cake.Common.IO;
using Cake.Common.Tools.DotNet.NuGet.Push;
using Cake.Common.Tools.DotNet;
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

[TaskName("build")]
[IsDependentOn(typeof(PackMumrichTemplatesTask))]
public class BuildTask : FrostingTask<BuildContext>
{
}

[TaskName("nuget-publish")]
[IsDependentOn(typeof(BuildTask))]
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