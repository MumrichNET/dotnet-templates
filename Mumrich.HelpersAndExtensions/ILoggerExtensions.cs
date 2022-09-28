using System;
using System.Runtime.CompilerServices;

using Microsoft.Extensions.Logging;

namespace Mumrich.HelpersAndExtensions
{
  public static class LoggerExtensions
  {
    public static void LogInformationWithCallerInfo(this ILogger logger, string message, [CallerMemberName] string callerMemberName = null)
    {
      logger.LogInformation($"{callerMemberName}: {message}");
    }

    public static void LogErrorWithCallerInfo(this ILogger logger, Exception exception,
      [CallerMemberName] string callerMemberName = null)
    {
      logger.LogError($"{callerMemberName}: {exception.GetFullMessage()}", exception);
    }

    public static void LogErrorWithCallerInfo(this ILogger logger, string message,
      [CallerMemberName] string callerMemberName = null)
    {
      logger.LogError($"{callerMemberName}: {message}");
    }
  }
}