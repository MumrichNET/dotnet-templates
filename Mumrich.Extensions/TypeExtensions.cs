using System;
using System.Collections.Generic;

namespace Mumrich.Extensions
{
  public static class TypeExtensions
  {
    public static bool IsList(this Type type)
    {
      return type.Name == typeof(List<object>).Name;
    }

    public static bool IsDictionary(this Type type)
    {
      return type.Name == typeof(Dictionary<object, object>).Name;
    }
  }
}
