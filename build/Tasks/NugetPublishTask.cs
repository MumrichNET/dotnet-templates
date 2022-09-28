using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.NuGet.Push;
using Cake.Core;
using Cake.Frosting;

namespace Build.Tasks;

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