using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LiteTube.StreamVideo.Pls
{
    public class PlsParser
    {
        private readonly Dictionary<int, PlsTrack> _tracks = new Dictionary<int, PlsTrack>();
        private bool _foundPlaylist;
        private int? _numberOfEntries;
        private PlsTrack[] _orderedTracks;

        public int? Version { get; private set; }

        public ICollection<PlsTrack> Tracks => _orderedTracks;

        public Uri BaseUrl { get; }

        public PlsParser(Uri url)
        {
            if (null == url)
                throw new ArgumentNullException(nameof(url));

            BaseUrl = url;
        }

        public async Task<bool> ParseAsync(TextReader tr)
        {
            _tracks.Clear();
            _numberOfEntries = new int?();
            Version = new int?();
            _foundPlaylist = false;
            _orderedTracks = null;
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
                while (59 == (int)firstChar || 35 == (int)firstChar);
                if (!_foundPlaylist)
                {
                    if (!string.Equals("[playlist]", line, StringComparison.OrdinalIgnoreCase))
                        Debug.WriteLine("PlsParser.Parser() invalid line while expecting parser: " + line);
                    else
                        _foundPlaylist = true;
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
                            HandleNumberOfEntries(str2, line);
                        else if (string.Equals("Version", str1, StringComparison.OrdinalIgnoreCase))
                            HandleVersion(str2, line);
                        else
                            HandleTrack(line, str1, str2);
                    }
                }
            }
            label_2:
            _orderedTracks = _tracks.OrderBy(kv => kv.Key).Select(kv => kv.Value).ToArray();
            if (_numberOfEntries.HasValue && _numberOfEntries.Value != _orderedTracks.Length)
                Debug.WriteLine("PlsParser.Parse() entries mismatch ({0} != {1})", _numberOfEntries, _orderedTracks.Length);
            return _foundPlaylist;
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
                if (!int.TryParse(key.Substring(startOfTrackNumber + 1, key.Length - (startOfTrackNumber + 1)), NumberStyles.Integer, (IFormatProvider)CultureInfo.InvariantCulture, out result1))
                {
                    Debug.WriteLine("PlsParser.HandleTrack() invalid track number: " + line);
                }
                else
                {
                    PlsTrack plsTrack;
                    if (!_tracks.TryGetValue(result1, out plsTrack))
                    {
                        plsTrack = new PlsTrack();
                        _tracks[result1] = plsTrack;
                    }
                    if (string.Equals("File", b))
                    {
                        if (null != plsTrack.File)
                            Debug.WriteLine("PlsParser.Parser() duplicate file property for entry {0}: {1}", (object)result1, (object)line);
                        else
                            plsTrack.File = value;
                    }
                    else if (string.Equals("Title", b))
                    {
                        if (null != plsTrack.Title)
                            Debug.WriteLine("PlsParser.Parser() duplicate title property for entry {0}: {1}", (object)result1, (object)line);
                        else
                            plsTrack.Title = value;
                    }
                    else if (string.Equals("Length", b))
                    {
                        if (plsTrack.Length.HasValue)
                        {
                            Debug.WriteLine("PlsParser.Parser() duplicate length property for entry {0}: {1}", (object)result1, (object)line);
                        }
                        else
                        {
                            Decimal result2;
                            if (!Decimal.TryParse(value, NumberStyles.Number, (IFormatProvider)CultureInfo.InvariantCulture, out result2))
                            {
                                Debug.WriteLine("PlsParser.Parser() invalid length property for entry {0}: {1}", (object)result1, (object)line);
                            }
                            else
                            {
                                try
                                {
                                    plsTrack.Length = new TimeSpan?(TimeSpan.FromSeconds((double)result2));
                                }
                                catch (InvalidCastException ex)
                                {
                                    Debug.WriteLine("PlsParser.Parser() invalid numeric length property for entry {0}: {1}", (object)result1, (object)line);
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
            if (int.TryParse(value, NumberStyles.Integer, (IFormatProvider)CultureInfo.InvariantCulture, out result))
            {
                if (Version.HasValue)
                    Debug.WriteLine("PlsParser.Parser() repeated version: " + line);
                else
                    Version = new int?(result);
            }
            else
                Debug.WriteLine("PlsParser.Parser() invalid NumberOfEntries: " + line);
        }

        private void HandleNumberOfEntries(string value, string line)
        {
            int result;
            if (int.TryParse(value, NumberStyles.Integer, (IFormatProvider)CultureInfo.InvariantCulture, out result))
            {
                if (_numberOfEntries.HasValue)
                    Debug.WriteLine("PlsParser.Parser() repeated NumberOfEntries: " + line);
                else
                    _numberOfEntries = new int?(result);
            }
            else
                Debug.WriteLine("PlsParser.Parser() invalid NumberOfEntries: " + line);
        }
    }
}
