using Cake.Common;
using Cake.Core;
using Cake.Frosting;

namespace Build;

public class BuildContext : FrostingContext
{
  public BuildContext(ICakeContext context)
    : base(context)
  {
    CleanBuildOutput = context.Argument(nameof(CleanBuildOutput), true);
    MsBuildConfiguration = context.Argument(nameof(MsBuildConfiguration), "Release");
    BuildOutputPath = context.Argument(nameof(BuildOutputPath), "../build-output");
    NugetOrgApiKey = context.Argument<string>(nameof(NugetOrgApiKey), null);
    NugetOrgSource = context.Argument(nameof(NugetOrgSource), "https://api.nuget.org/v3/index.json");
  }

  public string BuildOutputPath { get; }
  public bool CleanBuildOutput { get; }
  public string MsBuildConfiguration { get; }
  public string NugetOrgApiKey { get; }
  public string NugetOrgSource { get; }
}
