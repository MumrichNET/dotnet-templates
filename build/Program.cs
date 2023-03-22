using Build.Tasks;

using Cake.Frosting;

namespace Build;

public static class Program
{
  public static int Main(string[] args)
  {
    return new CakeHost()
      .UseContext<BuildContext>()
      .Run(args);
  }
}

[TaskName("build")]
[IsDependentOn(typeof(PackMumrichAkkaExtTask))]
[IsDependentOn(typeof(PackMumrichHelpersAndExtensionsTask))]
[IsDependentOn(typeof(PackMumrichSpaDevMiddlewareDomainTask))]
[IsDependentOn(typeof(PackMumrichSpaDevMiddlewareMsBuildTask))]
[IsDependentOn(typeof(PackMumrichSpaDevMiddlewareTask))]
[IsDependentOn(typeof(PackMumrichTemplatesTask))]
public class BuildTask : FrostingTask<BuildContext>
{
}
