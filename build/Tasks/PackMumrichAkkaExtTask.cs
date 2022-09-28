using Build.Bases;

namespace Build.Tasks;

public class PackMumrichAkkaExtTask : PackTaskBase
{
  public override void Run(BuildContext context)
  {
    const string csprojName = "Mumrich.AkkaExt";

    PackCsproj(context, csprojName);
  }
}
