using Cake.Frosting;

namespace Build.Tasks;

[TaskName("build")]
[IsDependentOn(typeof(PackMumrichSpaDevMiddlewareTask))]
[IsDependentOn(typeof(PackMumrichAkkaExtTask))]
[IsDependentOn(typeof(PackMumrichHelpersAndExtensionsTask))]
public class BuildTask : FrostingTask<BuildContext>
{
}