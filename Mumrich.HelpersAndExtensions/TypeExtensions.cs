using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Mumrich.HelpersAndExtensions
{
  public static class TypeExtensions
  {
    public static bool IsDictionary(this Type type)
    {
      return type.Name == typeof(Dictionary<object, object>).Name;
    }

    public static bool IsList(this Type type)
    {
      return type.Name == typeof(List<object>).Name;
    }

    public static IEnumerable<PropertyInfo> DeepLookupForWantedProperties(this Type type, params Type[] wantedTypes)
    {
      List<PropertyInfo> response = new();

      foreach (PropertyInfo prop in type.GetProperties())
      {
        if (wantedTypes.Contains(prop.PropertyType))
        {
          response.Add(prop);
        }
        else
        {
          response.AddRange(prop.PropertyType.DeepLookupForWantedProperties(wantedTypes));
        }
      }

      return response;
    }
  }
}