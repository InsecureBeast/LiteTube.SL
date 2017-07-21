// Decompiled with JetBrains decompiler
// Type: SM.Media.Pls.PlsParser
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SM.Media.Pls
{
  public class PlsParser
  {
    private readonly Dictionary<int, PlsTrack> _tracks = new Dictionary<int, PlsTrack>();
    private readonly Uri _url;
    private bool _foundPlaylist;
    private int? _numberOfEntries;
    private PlsTrack[] _orderedTracks;
    private int? _version;

    public int? Version
    {
      get
      {
        return this._version;
      }
    }

    public ICollection<PlsTrack> Tracks
    {
      get
      {
        return (ICollection<PlsTrack>) this._orderedTracks;
      }
    }

    public Uri BaseUrl
    {
      get
      {
        return this._url;
      }
    }

    public PlsParser(Uri url)
    {
      if ((Uri) null == url)
        throw new ArgumentNullException("url");
      this._url = url;
    }

    public async Task<bool> ParseAsync(TextReader tr)
    {
      this._tracks.Clear();
      this._numberOfEntries = new int?();
      this._version = new int?();
      this._foundPlaylist = false;
      this._orderedTracks = (PlsTrack[]) null;
      while (true)
      {
        string line;
        char firstChar;
        do
        {
          do
          {
            do
            {
              line = await tr.ReadLineAsync().ConfigureAwait(false);
              if (null == line)
                goto label_2;
            }
            while (string.IsNullOrWhiteSpace(line));
            line = line.Trim();
          }
          while (line.Length < 1);
          firstChar = line[0];
        }
        while (59 == (int) firstChar || 35 == (int) firstChar);
        if (!this._foundPlaylist)
        {
          if (!string.Equals("[playlist]", line, StringComparison.OrdinalIgnoreCase))
            Debug.WriteLine("PlsParser.Parser() invalid line while expecting parser: " + line);
          else
            this._foundPlaylist = true;
        }
        else
        {
          int length = line.IndexOf('=');
          if (length <= 0 || length + 1 >= line.Length)
          {
            Debug.WriteLine("PlsParser.Parser() invalid line: " + line);
          }
          else
          {
            string str1 = line.Substring(0, length).Trim();
            string str2 = line.Substring(length + 1, line.Length - (length + 1)).Trim();
            if (string.Equals("NumberOfEntries", str1, StringComparison.OrdinalIgnoreCase))
              this.HandleNumberOfEntries(str2, line);
            else if (string.Equals("Version", str1, StringComparison.OrdinalIgnoreCase))
              this.HandleVersion(str2, line);
            else
              this.HandleTrack(line, str1, str2);
          }
        }
      }
label_2:
      this._orderedTracks = Enumerable.ToArray<PlsTrack>(Enumerable.Select<KeyValuePair<int, PlsTrack>, PlsTrack>((IEnumerable<KeyValuePair<int, PlsTrack>>) Enumerable.OrderBy<KeyValuePair<int, PlsTrack>, int>((IEnumerable<KeyValuePair<int, PlsTrack>>) this._tracks, (Func<KeyValuePair<int, PlsTrack>, int>) (kv => kv.Key)), (Func<KeyValuePair<int, PlsTrack>, PlsTrack>) (kv => kv.Value)));
      if (this._numberOfEntries.HasValue && this._numberOfEntries.Value != this._orderedTracks.Length)
        Debug.WriteLine("PlsParser.Parse() entries mismatch ({0} != {1})", (object) this._numberOfEntries, (object) this._orderedTracks.Length);
      return this._foundPlaylist;
    }

    private static int FindStartOfTrackNumber(string key)
    {
      for (int index = key.Length - 1; index >= 0; --index)
      {
        if (!char.IsDigit(key[index]))
        {
          if (index < key.Length - 1)
            return index;
          break;
        }
      }
      return -1;
    }

    private void HandleTrack(string line, string key, string value)
    {
      int startOfTrackNumber = PlsParser.FindStartOfTrackNumber(key);
      if (startOfTrackNumber < 0)
      {
        Debug.WriteLine("PlsParser.HandleTrack() unable to find track number: " + line);
      }
      else
      {
        string b = key.Substring(0, startOfTrackNumber + 1).Trim();
        int result1;
        if (!int.TryParse(key.Substring(startOfTrackNumber + 1, key.Length - (startOfTrackNumber + 1)), NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result1))
        {
          Debug.WriteLine("PlsParser.HandleTrack() invalid track number: " + line);
        }
        else
        {
          PlsTrack plsTrack;
          if (!this._tracks.TryGetValue(result1, out plsTrack))
          {
            plsTrack = new PlsTrack();
            this._tracks[result1] = plsTrack;
          }
          if (string.Equals("File", b))
          {
            if (null != plsTrack.File)
              Debug.WriteLine("PlsParser.Parser() duplicate file property for entry {0}: {1}", (object) result1, (object) line);
            else
              plsTrack.File = value;
          }
          else if (string.Equals("Title", b))
          {
            if (null != plsTrack.Title)
              Debug.WriteLine("PlsParser.Parser() duplicate title property for entry {0}: {1}", (object) result1, (object) line);
            else
              plsTrack.Title = value;
          }
          else if (string.Equals("Length", b))
          {
            if (plsTrack.Length.HasValue)
            {
              Debug.WriteLine("PlsParser.Parser() duplicate length property for entry {0}: {1}", (object) result1, (object) line);
            }
            else
            {
              Decimal result2;
              if (!Decimal.TryParse(value, NumberStyles.Number, (IFormatProvider) CultureInfo.InvariantCulture, out result2))
              {
                Debug.WriteLine("PlsParser.Parser() invalid length property for entry {0}: {1}", (object) result1, (object) line);
              }
              else
              {
                try
                {
                  plsTrack.Length = new TimeSpan?(TimeSpan.FromSeconds((double) result2));
                }
                catch (InvalidCastException ex)
                {
                  Debug.WriteLine("PlsParser.Parser() invalid numeric length property for entry {0}: {1}", (object) result1, (object) line);
                }
              }
            }
          }
          else
            Debug.WriteLine("PlsParser.Parse() unknown property: " + line);
        }
      }
    }

    private void HandleVersion(string value, string line)
    {
      int result;
      if (int.TryParse(value, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result))
      {
        if (this._version.HasValue)
          Debug.WriteLine("PlsParser.Parser() repeated version: " + line);
        else
          this._version = new int?(result);
      }
      else
        Debug.WriteLine("PlsParser.Parser() invalid NumberOfEntries: " + line);
    }

    private void HandleNumberOfEntries(string value, string line)
    {
      int result;
      if (int.TryParse(value, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result))
      {
        if (this._numberOfEntries.HasValue)
          Debug.WriteLine("PlsParser.Parser() repeated NumberOfEntries: " + line);
        else
          this._numberOfEntries = new int?(result);
      }
      else
        Debug.WriteLine("PlsParser.Parser() invalid NumberOfEntries: " + line);
    }
  }
}
