using Build.Bases;

namespace Build.Tasks;

public class PackMumrichTemplatesTask : PackTaskBase
{
  public override void Run(BuildContext context)
  {
    const string csprojName = "Mumrich.Templates";

    PackCsproj(context, csprojName);
  }
}