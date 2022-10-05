namespace Mumrich.SpaDevMiddleware.Helpers
{
  public static class AppPathHelper
  {
    public static string GetValidIntermediateAppPath(string appPath)
    {
      if (string.IsNullOrWhiteSpace(appPath))
      {
        appPath = "/";
      }
      else
      {
        if (!appPath.StartsWith("/"))
        {
          appPath = $"/{appPath}";
        }

        if (!appPath.EndsWith("/"))
        {
          appPath = $"{appPath}/";
        }
      }

      return appPath;
    }
  }
}