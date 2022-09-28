using System.IO;

namespace Mumrich.HelpersAndExtensions.Helpers
{
  public static class DirPathHelper
  {
    /// <summary>
    /// Unifies the Path-Methods 'GetFullPath' and 'Combine' for simplification, as they are often used
    /// </summary>
    /// <param name="path1"></param>
    /// <param name="path2"></param>
    /// <returns>the fully combined path</returns>
    public static string CombineToFullPath(string path1, string path2)
    {
      return Path.GetFullPath(Path.Combine(path1, path2));
    }
  }
}