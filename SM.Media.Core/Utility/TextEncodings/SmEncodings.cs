using System;
using System.Diagnostics;
using System.Text;

namespace SM.Media.Core.Utility.TextEncodings
{
    public class SmEncodings : ISmEncodings
    {
        internal static readonly Encoding Ascii = GetAsciiEncoding();
        internal static readonly Encoding Latin1 = GetLatin1Encoding();

        public Encoding Latin1Encoding
        {
            get { return Latin1; }
        }

        public Encoding AsciiEncoding
        {
            get { return Ascii; }
        }

        private static Encoding GetLatin1Encoding()
        {
            Encoding encoding1 = GetEncoding("Windows-1252");
            if (null != encoding1)
                return encoding1;

            //Encoding encoding2 = GetEncoding("iso-8859-1");
            Encoding encoding2 = GetEncoding("UTF-8");
            if (null != encoding2)
                return encoding2;

            return new Windows1252Encoding();
        }

        private static Encoding GetAsciiEncoding()
        {
            Encoding encoding = GetEncoding("us-ascii");
            if (null != encoding)
                return encoding;

            return new AsciiEncoding();
        }

        private static Encoding GetEncoding(string name)
        {
            try
            {
                return Encoding.GetEncoding(name);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to get " + name + " encoding, ex = " + ex.Message);
            }

            return null;
        }
    }
}
