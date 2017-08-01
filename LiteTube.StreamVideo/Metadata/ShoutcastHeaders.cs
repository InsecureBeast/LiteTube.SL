using System;
using System.Collections.Generic;
using System.Linq;

namespace LiteTube.StreamVideo.Metadata
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
            var dic = new Dictionary<string, int>();
          if (keyValuePair.Value == null)
          {
            dic = new Dictionary<string, int>(7)
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
          
          if (dic.TryGetValue(key, out num))
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
