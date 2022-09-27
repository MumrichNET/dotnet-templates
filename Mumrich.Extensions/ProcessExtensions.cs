using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;

using Mumrich.Extensions.WMI;

namespace Mumrich.Extensions;

public static class ProcessExtensions
{
  public static Process GetParentProcess(this Process process)
  {
    if (OperatingSystem.IsWindows())
    {
      ManagementObject currentProc = new ManagementObjectSearcher($"Select * From Win32_Process Where ProcessId={process.Id}").GetManagementObjects().FirstOrDefault();

      if (currentProc != default)
      {
        return Process.GetProcessById(currentProc.GetParentProcessId());
      }
    }

    return null;
  }

  public static IEnumerable<Process> GetChildProcesses(this Process process)
  {
    if (OperatingSystem.IsWindows())
    {
      List<Process> children = new();
      foreach (ManagementObject mo in new ManagementObjectSearcher($"Select * From Win32_Process Where ParentProcessID={process.Id}").GetManagementObjects())
      {
        children.Add(Process.GetProcessById(mo.GetProcessId()));
      }
      return children;
    }

    return null;
  }

  public static bool IsVisualStudioDebugger(this Process process)
  {
    return process.ProcessName.Contains("VsDebugConsole");
  }

  public static bool IsVsCodeDebugger(this Process process)
  {
    return process.ProcessName.Contains("vsdbg-ui");
  }
}