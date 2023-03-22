using Build.Bases;

namespace Build.Tasks;

public class PackMumrichSpaDevMiddlewareMsBuildTask : PackTaskBase
{
  public override void Run(BuildContext context)
  {
    const string csprojName = "Mumrich.SpaDevMiddleware.MsBuild";

    PackCsproj(context, csprojName);
  }
}