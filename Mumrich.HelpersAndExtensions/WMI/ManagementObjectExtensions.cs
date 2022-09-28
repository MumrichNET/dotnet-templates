using System;
using System.Management;

using Mumrich.HelpersAndExtensions.Exceptions;

namespace Mumrich.HelpersAndExtensions.WMI;

public static class ManagementObjectExtensions
{
  public static int GetParentProcessId(this ManagementObject mo)
  {
    return OperatingSystem.IsWindows()
      ? Convert.ToInt32(mo["ParentProcessID"])
      : throw new UnsupportedOSException("Windows only!");
  }

  public static int GetProcessId(this ManagementObject mo)
  {
    return OperatingSystem.IsWindows()
      ? Convert.ToInt32(mo["ProcessID"])
      : throw new UnsupportedOSException("Windows only!");
  }
}