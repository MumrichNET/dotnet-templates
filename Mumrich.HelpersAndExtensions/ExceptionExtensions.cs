using System;
using System.Runtime.CompilerServices;

namespace Mumrich.HelpersAndExtensions
{
  public static class ExceptionExtensions
  {
    public static string GetFullMessage(this Exception exception, [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
    {
      string messageBase = $"{sourceFilePath} ({sourceLineNumber})";

      return $"{messageBase}: {exception}";
    }
  }
}