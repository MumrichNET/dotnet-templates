using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;

using Extensions;

using Helpers;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace GenericEntity
{
  public abstract class GenericEntityBase
  {
    private readonly Regex _enumerableExpr = new(@"(.+)\[(\d)\]");
    protected abstract ILogger Logger { get; }

    protected bool TrySetProperty<TValue>(string[] propertyNamespaceParts, TValue rawValue, object targetObject)
    {
      try
      {
        if (targetObject == null)
        {
          throw new NullReferenceException($"{nameof(targetObject)} must not be null");
        }

        if (propertyNamespaceParts == null || propertyNamespaceParts.Length == 0)
        {
          Logger.LogErrorWithCallerInfo($"invalid {nameof(propertyNamespaceParts)}: '{propertyNamespaceParts}'");

          return false;
        }

        var topLevelPropName = propertyNamespaceParts[0].ToLowerInvariant();
        int? enumerableIndex = null;

        if (string.IsNullOrWhiteSpace(topLevelPropName))
        {
          Logger.LogErrorWithCallerInfo($"invalid {nameof(topLevelPropName)}: '{topLevelPropName}'");

          return false;
        }

        var match = _enumerableExpr.Match(topLevelPropName);

        if (match.Success)
        {
          topLevelPropName = match.Groups[1].Value;
          enumerableIndex = int.Parse(match.Groups[2].Value);
        }

        var topLevelProperty = Array.Find(targetObject.GetType().GetProperties(), p => p.Name.ToLowerInvariant() == topLevelPropName);

        if (topLevelProperty == null)
        {
          Logger.LogErrorWithCallerInfo($"invalid {nameof(topLevelProperty)}: '{null}'");

          return false;
        }

        if (propertyNamespaceParts.Length == 1)
        {
          return TrySetValue(topLevelProperty, targetObject, rawValue);
        }

        var newTargetObject = GetNewTargetObject(topLevelProperty, targetObject, enumerableIndex);

        return TrySetProperty(propertyNamespaceParts.Skip(1).ToArray(), rawValue, newTargetObject);
      }
      catch (Exception exception)
      {
        Logger.LogErrorWithCallerInfo(exception);
      }

      return false;
    }

    private static object GetNewTargetObject(PropertyInfo topLevelProperty, object targetObject, int? enumerableIndex)
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

    private bool TryInstantiatingTheRightType(PropertyInfo info, string serializedJsonValue, out object actualValue)
    {
      Logger.LogInformationWithCallerInfo($"'{serializedJsonValue}' -> '{info.PropertyType.FullName}'");

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
        Logger.LogErrorWithCallerInfo(exception);
        actualValue = null;
      }

      return false;
    }

    private bool TrySetValue<TValue>(PropertyInfo info, object instance, TValue rawValue)
    {
      try
      {
        if (rawValue is string stringValue && TryInstantiatingTheRightType(info, stringValue, out var targetValue))
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
          if (!TrySetValueFromJsonElement(info, instance, jsonElement))
          {
            info.SetValue(instance, rawValue);
          }

          return true;
        }

        return false;
      }
      catch (Exception exception)
      {
        Logger.LogErrorWithCallerInfo(exception);

        return false;
      }
    }

    private bool TrySetValueFromJsonElement(PropertyInfo info, object instance, JsonElement? jsonElement)
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
          Logger.LogInformationWithCallerInfo($"{info.Name} ({valueKind}) = '{jsonElement}'");
          info.SetValue(instance, jsonElement.Value.Deserialize(info.PropertyType));
          return true;

        case JsonValueKind.String:
          string serializedJsonValue = jsonElement.Value.GetString();

          if (TryInstantiatingTheRightType(info, serializedJsonValue, out object actualValue))
          {
            Logger.LogInformationWithCallerInfo($"{info.Name} ({valueKind}) = '{jsonElement}'");

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
