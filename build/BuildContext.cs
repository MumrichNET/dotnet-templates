using Cake.Common;
using Cake.Core;
using Cake.Frosting;

namespace Build;

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
