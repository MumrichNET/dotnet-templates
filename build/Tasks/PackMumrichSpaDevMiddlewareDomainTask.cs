using Build.Bases;

namespace Build.Tasks;

public class PackMumrichSpaDevMiddlewareDomainTask : PackTaskBase
{
  public override void Run(BuildContext context)
  {
    const string csprojName = "Mumrich.SpaDevMiddleware.Domain";

    PackCsproj(context, csprojName);
  }
}