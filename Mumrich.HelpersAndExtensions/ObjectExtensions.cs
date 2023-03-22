using System;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.Extensions.Logging;

namespace Mumrich.HelpersAndExtensions
{
  public static partial class ObjectExtensions
  {
    public static bool TrySetProperty<TValue>(this object targetObject, string propertyNamespace, TValue rawValue, ILogger logger, char namespaceSeparator = '.')
    {
      return targetObject.TrySetProperty(propertyNamespace.Split(namespaceSeparator).ToArray(), rawValue, logger);
    }

    [GeneratedRegex("(.+)\\[(\\d)\\]")]
    private static partial Regex EnumerableExpr();

    private static bool TrySetProperty<TValue>(this object targetObject, string[] propertyNamespaceParts, TValue rawValue, ILogger logger)
    {
      try
      {
        if (targetObject == null)
        {
          throw new NullReferenceException($"{nameof(targetObject)} must not be null");
        }

        if (propertyNamespaceParts == null || propertyNamespaceParts.Length == 0)
        {
          logger.LogErrorWithCallerInfo($"invalid {nameof(propertyNamespaceParts)}: '{propertyNamespaceParts}'");

          return false;
        }

        var topLevelPropName = propertyNamespaceParts[0].ToLowerInvariant();
        int? enumerableIndex = null;

        if (string.IsNullOrWhiteSpace(topLevelPropName))
        {
          logger.LogErrorWithCallerInfo($"invalid {nameof(topLevelPropName)}: '{topLevelPropName}'");

          return false;
        }

        var match = EnumerableExpr().Match(topLevelPropName);

        if (match.Success)
        {
          topLevelPropName = match.Groups[1].Value;
          enumerableIndex = int.Parse(match.Groups[2].Value);
        }

        var topLevelProperty = Array.Find(targetObject.GetType().GetProperties(), p => p.Name.ToLowerInvariant() == topLevelPropName);

        if (topLevelProperty == null)
        {
          logger.LogErrorWithCallerInfo($"invalid {nameof(topLevelProperty)}: '{null}'");

          return false;
        }

        if (propertyNamespaceParts.Length == 1)
        {
          return topLevelProperty.TrySetValue(targetObject, rawValue, logger);
        }

        var newTargetObject = topLevelProperty.GetNewTargetObject(targetObject, enumerableIndex);

        return newTargetObject.TrySetProperty(propertyNamespaceParts.Skip(1).ToArray(), rawValue, logger);
      }
      catch (Exception exception)
      {
        logger.LogErrorWithCallerInfo(exception);
      }

      return false;
    }
  }
}