// Decompiled with JetBrains decompiler
// Type: SM.Media.Metadata.ShoutcastHeaders
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace SM.Media.Metadata
{
  internal class ShoutcastHeaders
  {
    private readonly int? _bitrate;
    private readonly string _description;
    private readonly string _genre;
    private readonly int? _metaInterval;
    private readonly string _name;
    private readonly bool _supportsIcyMetadata;
    private readonly Uri _website;

    public int? Bitrate
    {
      get
      {
        return this._bitrate;
      }
    }

    public string Description
    {
      get
      {
        return this._description;
      }
    }

    public string Genre
    {
      get
      {
        return this._genre;
      }
    }

    public int? MetaInterval
    {
      get
      {
        return this._metaInterval;
      }
    }

    public string Name
    {
      get
      {
        return this._name;
      }
    }

    public bool SupportsIcyMetadata
    {
      get
      {
        return this._supportsIcyMetadata;
      }
    }

    public Uri Website
    {
      get
      {
        return this._website;
      }
    }

    public ShoutcastHeaders(Uri streamUrl, IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers)
    {
      foreach (KeyValuePair<string, IEnumerable<string>> keyValuePair in headers)
      {
        string key = keyValuePair.Key.ToLowerInvariant();
        if (key != null)
        {
          // ISSUE: reference to a compiler-generated field
          if (\u003CPrivateImplementationDetails\u003E\u007B36CDA6C8\u002D9742\u002D4B9A\u002D8F0F\u002D25CFBA3563E6\u007D.\u0024\u0024method0x6000231\u002D1 == null)
          {
            // ISSUE: reference to a compiler-generated field
            \u003CPrivateImplementationDetails\u003E\u007B36CDA6C8\u002D9742\u002D4B9A\u002D8F0F\u002D25CFBA3563E6\u007D.\u0024\u0024method0x6000231\u002D1 = new Dictionary<string, int>(7)
            {
              {
                "icy-br",
                0
              },
              {
                "icy-description",
                1
              },
              {
                "icy-genre",
                2
              },
              {
                "icy-metadata",
                3
              },
              {
                "icy-metaint",
                4
              },
              {
                "icy-name",
                5
              },
              {
                "icy-url",
                6
              }
            };
          }
          int num;
          // ISSUE: reference to a compiler-generated field
          // ISSUE: explicit non-virtual call
          if (__nonvirtual (\u003CPrivateImplementationDetails\u003E\u007B36CDA6C8\u002D9742\u002D4B9A\u002D8F0F\u002D25CFBA3563E6\u007D.\u0024\u0024method0x6000231\u002D1.TryGetValue(key, out num)))
          {
            switch (num)
            {
              case 0:
                using (IEnumerator<string> enumerator = keyValuePair.Value.GetEnumerator())
                {
                  while (enumerator.MoveNext())
                  {
                    int result;
                    if (int.TryParse(enumerator.Current, out result) && result > 0)
                    {
                      this._bitrate = new int?(result * 1000);
                      break;
                    }
                  }
                  break;
                }
              case 1:
                this._description = Enumerable.FirstOrDefault<string>(keyValuePair.Value);
                break;
              case 2:
                this._genre = Enumerable.FirstOrDefault<string>(keyValuePair.Value);
                break;
              case 3:
                this._supportsIcyMetadata = true;
                break;
              case 4:
                using (IEnumerator<string> enumerator = keyValuePair.Value.GetEnumerator())
                {
                  while (enumerator.MoveNext())
                  {
                    int result;
                    if (int.TryParse(enumerator.Current, out result) && result > 0)
                    {
                      this._metaInterval = new int?(result);
                      break;
                    }
                  }
                  break;
                }
              case 5:
                this._name = Enumerable.FirstOrDefault<string>(keyValuePair.Value);
                break;
              case 6:
                using (IEnumerator<string> enumerator = keyValuePair.Value.GetEnumerator())
                {
                  while (enumerator.MoveNext())
                  {
                    string current = enumerator.Current;
                    Uri result;
                    if (Uri.TryCreate(streamUrl, current, out result))
                    {
                      this._website = result;
                      break;
                    }
                  }
                  break;
                }
            }
          }
        }
      }
    }
  }
}
