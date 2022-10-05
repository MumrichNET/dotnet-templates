using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text.Json;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using Mumrich.HelpersAndExtensions.Helpers;

namespace Mumrich.HelpersAndExtensions
{
  public static class PropertyInfoExtensions
  {
    public static object GetNewTargetObject(this PropertyInfo topLevelProperty, object targetObject, int? enumerableIndex)
    {
      var newTargetObject = topLevelProperty.GetValue(targetObject);

      if (topLevelProperty.PropertyType.IsList())
      {
        if (newTargetObject == null)
        {
          var newList = Activator.CreateInstance(topLevelProperty.PropertyType);
          topLevelProperty.SetValue(targetObject, newList);
          newTargetObject = topLevelProperty.GetValue(targetObject);
        }

        var listType = topLevelProperty.PropertyType;
        var listLength = (int)newTargetObject.GetType().GetProperty("Count").GetValue(newTargetObject);

        if (listLength - 1 < enumerableIndex.Value)
        {
          var addMethod = listType.GetMethod("Add");
          var innerType = listType.GenericTypeArguments[0];
          var newInstance = Activator.CreateInstance(innerType);
          addMethod.Invoke(newTargetObject, new[] { newInstance });
          newTargetObject = newInstance;
        }
        else
        {
          var indexerProperty = newTargetObject.GetType().GetProperty("Item");

          newTargetObject = indexerProperty.GetValue(newTargetObject, new object[] { enumerableIndex.Value });
        }
      }

      if (topLevelProperty.PropertyType.IsDictionary())
      {
        ExceptionHelper.ThrowNewTimeToImplementException("IsDictionary");
      }

      return newTargetObject;
    }

    public static bool TryInstantiatingTheRightType(this PropertyInfo info, string serializedJsonValue, out object actualValue, ILogger logger)
    {
      logger.LogInformationWithCallerInfo($"'{serializedJsonValue}' -> '{info.PropertyType.FullName}'");

      if (info.PropertyType.IsEnum)
      {
        actualValue = Enum.Parse(info.PropertyType, serializedJsonValue);

        return true;
      }

      if (
        info.PropertyType.FullName == typeof(DateTime).FullName ||
        info.PropertyType.FullName == typeof(DateTime?).FullName)
      {
        const string format = "dd.MM.yyyy";

        actualValue = DateTime.ParseExact(serializedJsonValue, format, CultureInfo.InvariantCulture);

        return true;
      }

      if (info.PropertyType.FullName == typeof(Guid).FullName || info.PropertyType.FullName == typeof(Guid?).FullName)
      {
        actualValue = Guid.Parse(serializedJsonValue);

        return true;
      }

      try
      {
        actualValue = Convert.ChangeType(serializedJsonValue, info.PropertyType);

        return true;
      }
      catch (Exception exception)
      {
        logger.LogErrorWithCallerInfo(exception);
        actualValue = null;
      }

      return false;
    }

    public static bool TrySetValue<TValue>(this PropertyInfo info, object instance, TValue rawValue, ILogger logger)
    {
      try
      {
        if (rawValue is string stringValue && info.TryInstantiatingTheRightType(stringValue, out var targetValue, logger))
        {
          info.SetValue(instance, targetValue);
          return true;
        }

        if (rawValue is List<FormFile> files)
        {
          info.SetValue(instance, files);
          return true;
        }

        if (rawValue is JsonElement jsonElement)
        {
          if (!info.TrySetValueFromJsonElement(instance, jsonElement, logger))
          {
            info.SetValue(instance, rawValue);
          }

          return true;
        }

        return false;
      }
      catch (Exception exception)
      {
        logger.LogErrorWithCallerInfo(exception);

        return false;
      }
    }

    public static bool TrySetValueFromJsonElement(this PropertyInfo info, object instance, JsonElement? jsonElement, ILogger logger)
    {
      if (!jsonElement.HasValue)
      {
        return false;
      }

      var valueKind = jsonElement.Value.ValueKind;

      switch (valueKind)
      {
        case JsonValueKind.Object:
        case JsonValueKind.Number:
        case JsonValueKind.True:
        case JsonValueKind.False:
          logger.LogInformationWithCallerInfo($"{info.Name} ({valueKind}) = '{jsonElement}'");
          info.SetValue(instance, jsonElement.Value.Deserialize(info.PropertyType));
          return true;

        case JsonValueKind.String:
          string serializedJsonValue = jsonElement.Value.GetString();

          if (info.TryInstantiatingTheRightType(serializedJsonValue, out object actualValue, logger))
          {
            logger.LogInformationWithCallerInfo($"{info.Name} ({valueKind}) = '{jsonElement}'");

            info.SetValue(instance, actualValue);

            return true;
          }
          break;

        default:
          ExceptionHelper.ThrowNewTimeToImplementException(valueKind.ToString());
          break;
      }

      return false;
    }
  }
}