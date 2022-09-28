using Build.Bases;

namespace Build.Tasks;

public class PackMumrichSpaDevMiddlewareTask : PackTaskBase
{
  public override void Run(BuildContext context)
  {
    const string csprojName = "Mumrich.SpaDevMiddleware";

    PackCsproj(context, csprojName);
  }
}
