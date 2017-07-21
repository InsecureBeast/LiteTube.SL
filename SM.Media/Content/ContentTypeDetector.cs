// Decompiled with JetBrains decompiler
// Type: SM.Media.Content.ContentTypeDetector
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Web;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SM.Media.Content
{
  public class ContentTypeDetector : IContentTypeDetector
  {
    protected static readonly ContentType[] NoContent = new ContentType[0];
    protected readonly ContentType[] ContentTypes;
    protected readonly ILookup<string, ContentType> ExtensionLookup;
    protected readonly ILookup<string, ContentType> MimeLookup;

    public ContentTypeDetector(IEnumerable<ContentType> contentTypes)
    {
      if (null == contentTypes)
        throw new ArgumentNullException("contentTypes");
      this.ContentTypes = Enumerable.ToArray<ContentType>(contentTypes);
      this.ExtensionLookup = Enumerable.ToLookup(Enumerable.SelectMany((IEnumerable<ContentType>) this.ContentTypes, (Func<ContentType, IEnumerable<string>>) (ct => (IEnumerable<string>) ct.FileExts), (ct, ext) =>
      {
        var fAnonymousType2 = new
        {
          ext = ext,
          ContentType = ct
        };
        return fAnonymousType2;
      }), arg => arg.ext, x => x.ContentType, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      IEnumerable<\u003C\u003Ef__AnonymousType3<string, ContentType>> second = Enumerable.Select((IEnumerable<ContentType>) this.ContentTypes, ct =>
      {
        var fAnonymousType3 = new
        {
          MimeType = ct.MimeType,
          ContentType = ct
        };
        return fAnonymousType3;
      });
      this.MimeLookup = Enumerable.ToLookup(Enumerable.Union(Enumerable.SelectMany(Enumerable.Where<ContentType>((IEnumerable<ContentType>) this.ContentTypes, (Func<ContentType, bool>) (ct => null != ct.AlternateMimeTypes)), (Func<ContentType, IEnumerable<string>>) (ct => (IEnumerable<string>) ct.AlternateMimeTypes), (ct, mime) =>
      {
        var fAnonymousType3 = new
        {
          MimeType = mime,
          ContentType = ct
        };
        return fAnonymousType3;
      }), second), arg => arg.MimeType, x => x.ContentType, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public virtual ICollection<ContentType> GetContentType(Uri url, string mimeType = null, string fileName = null)
    {
      if ((Uri) null == url)
        throw new ArgumentNullException("url");
      ICollection<ContentType> contentTypeByUrl = this.GetContentTypeByUrl(url);
      if (contentTypeByUrl != null && Enumerable.Any<ContentType>((IEnumerable<ContentType>) contentTypeByUrl))
        return contentTypeByUrl;
      if (null != mimeType)
      {
        ICollection<ContentType> byContentHeaders = this.GetContentTypeByContentHeaders(mimeType);
        if (null != byContentHeaders)
          return byContentHeaders;
      }
      if (string.IsNullOrWhiteSpace(fileName))
        return (ICollection<ContentType>) ContentTypeDetector.NoContent;
      return this.GetContentTypeByFileName(fileName) ?? (ICollection<ContentType>) ContentTypeDetector.NoContent;
    }

    protected virtual ICollection<ContentType> GetContentTypeByUrl(Uri url)
    {
      string extension = UriExtensions.GetExtension(url);
      if (null == extension)
        return (ICollection<ContentType>) null;
      return (ICollection<ContentType>) Enumerable.ToArray<ContentType>(this.ExtensionLookup[extension]);
    }

    protected virtual ICollection<ContentType> GetContentTypeByContentHeaders(string mimeType)
    {
      if (null == mimeType)
        return (ICollection<ContentType>) null;
      return (ICollection<ContentType>) Enumerable.ToArray<ContentType>(this.MimeLookup[mimeType]);
    }

    protected virtual ICollection<ContentType> GetContentTypeByFileName(string filename)
    {
      if (string.IsNullOrWhiteSpace(filename))
        return (ICollection<ContentType>) null;
      return (ICollection<ContentType>) Enumerable.ToArray<ContentType>(this.ExtensionLookup[UriExtensions.GetExtension(filename)]);
    }
  }
}
