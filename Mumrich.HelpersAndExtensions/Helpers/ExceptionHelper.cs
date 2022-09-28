using System;
using System.Runtime.CompilerServices;

namespace Mumrich.HelpersAndExtensions.Helpers
{
  public static class ExceptionHelper
  {
    public static void ThrowNewTimeToImplementException(
      string featureName,
      [CallerMemberName] string callerMemberName = null,
      [CallerLineNumber] int callerLineNumber = 0)
    {
#pragma warning disable RCS1079 // Throwing of new NotImplementedException.
      throw new NotImplementedException($"{callerMemberName}->{callerLineNumber}: Time to Implement '{featureName}'...");
#pragma warning restore RCS1079 // Throwing of new NotImplementedException.
    }
  }
}