// Decompiled with JetBrains decompiler
// Type: SM.Media.Utility.ExceptionExtensions
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;
using System.Text;

namespace SM.Media.Utility
{
  public static class ExceptionExtensions
  {
    public static string ExtendedMessage(this Exception ex)
    {
      if (null == ex)
        throw new ArgumentNullException("ex");
      if (null == ex.InnerException && null == ex as AggregateException)
        return string.IsNullOrEmpty(ex.Message) ? ex.GetType().FullName : ex.Message;
      StringBuilder sb = new StringBuilder();
      ExceptionExtensions.DumpException(sb, 0, ex);
      return sb.ToString();
    }

    private static void DumpException(StringBuilder sb, int indent, AggregateException aggregateException)
    {
      ExceptionExtensions.AppendWithIndent(sb, indent, (Exception) aggregateException);
      foreach (Exception exception in aggregateException.InnerExceptions)
        ExceptionExtensions.DumpException(sb, indent + 1, exception);
    }

    private static void DumpException(StringBuilder sb, int indent, Exception exception)
    {
      AggregateException aggregateException = exception as AggregateException;
      if (null != aggregateException)
        ExceptionExtensions.DumpException(sb, indent, aggregateException);
      else
        ExceptionExtensions.AppendWithIndent(sb, indent, exception);
    }

    private static void AppendWithIndent(StringBuilder sb, int indent, Exception exception)
    {
      sb.Append(' ', indent * 3);
      sb.AppendLine(exception.GetType().FullName + ": " + exception.Message);
      if (null == exception.InnerException)
        return;
      ExceptionExtensions.DumpException(sb, indent + 1, exception.InnerException);
    }
  }
}
