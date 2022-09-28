using System;
using System.Management;

using Mumrich.Extensions.Exceptions;

namespace Mumrich.Extensions.WMI;

public static class ManagementObjectExtensions
{
  public static int GetProcessId(this ManagementObject mo)
  {
    return OperatingSystem.IsWindows()
      ? Convert.ToInt32(mo["ProcessID"])
      : throw new UnsupportedOSException("Windows only!");
  }

  public static int GetParentProcessId(this ManagementObject mo)
  {
    return OperatingSystem.IsWindows()
      ? Convert.ToInt32(mo["ParentProcessID"])
      : throw new UnsupportedOSException("Windows only!");
  }
}