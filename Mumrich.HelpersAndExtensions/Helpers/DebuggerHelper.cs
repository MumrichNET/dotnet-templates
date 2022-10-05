using System.Diagnostics;

namespace Mumrich.HelpersAndExtensions.Helpers;

public static class DebuggerHelper
{
  public static bool IsRunByVisualStudio()
  {
    if (Debugger.IsAttached)
    {
      Process self = Process.GetCurrentProcess();
      Process parent = self.GetParentProcess();

      if (parent.IsVisualStudioDebugger())
      {
        return true;
      }
    }

    return false;
  }

  public static bool IsRunByVsCode()
  {
    if (Debugger.IsAttached)
    {
      Process self = Process.GetCurrentProcess();
      Process parent = self.GetParentProcess();

      if (parent.IsVsCodeDebugger())
      {
        return true;
      }
    }

    return false;
  }
}