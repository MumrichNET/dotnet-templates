using System;
using System.Runtime.Serialization;

namespace Mumrich.Extensions.Exceptions;

public class UnsupportedOSException : Exception
{
  public UnsupportedOSException()
  {
  }

  public UnsupportedOSException(string message) : base(message)
  {
  }

  public UnsupportedOSException(string message, Exception innerException) : base(message, innerException)
  {
  }

  protected UnsupportedOSException(SerializationInfo info, StreamingContext context) : base(info, context)
  {
  }
}
