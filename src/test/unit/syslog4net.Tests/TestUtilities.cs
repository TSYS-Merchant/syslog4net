using System.IO;
using System.Text;

namespace syslog4net.Tests
{
    public static class TestUtilities
    {
        public static string GetStringFromStream(Stream stream)
        {
            stream.Position = 0;
            var reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }

        public static string MakePrintableASCII(string str, int maxLength)
        {
            StringBuilder printableAscii = new StringBuilder();
            foreach (char ch in str)
            {
                if (ch > 32 && ch < 128)
                {
                    printableAscii.Append(ch);
                }
            }

            if (printableAscii.Length > maxLength)
            {
                return printableAscii.ToString().Substring(0, maxLength);
            }

            return printableAscii.ToString();        
        }

    }
}
