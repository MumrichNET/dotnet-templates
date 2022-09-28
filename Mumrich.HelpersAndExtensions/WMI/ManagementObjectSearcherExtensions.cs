using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

using Mumrich.HelpersAndExtensions.Exceptions;

namespace Mumrich.HelpersAndExtensions.WMI;

public static class ManagementObjectSearcherExtensions
{
  public static IEnumerable<ManagementObject> GetManagementObjects(this ManagementObjectSearcher searcher)
  {
    return OperatingSystem.IsWindows()
      ? searcher.Get().Cast<ManagementObject>()
      : throw new UnsupportedOSException("Windows only!");
  }
}