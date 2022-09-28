using Build.Bases;

namespace Build.Tasks;

public class PackMumrichHelpersAndExtensionsTask : PackTaskBase
{
  public override void Run(BuildContext context)
  {
    const string csprojName = "Mumrich.HelpersAndExtensions";

    PackCsproj(context, csprojName);
  }
}
