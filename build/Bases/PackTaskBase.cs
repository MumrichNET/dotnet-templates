using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Pack;
using Cake.Frosting;

namespace Build.Bases;

public abstract class PackTaskBase : FrostingTask<BuildContext>
{
  protected static void PackCsproj(BuildContext context, string csprojName, bool includeSource = false)
  {
    context.DotNetPack($"../{csprojName}/{csprojName}.csproj", new DotNetPackSettings
    {
      Configuration = context.MsBuildConriguration,
      NoLogo = true,
      IncludeSource = includeSource,
      OutputDirectory = context.BuildOutputPath
    });
  }
}
