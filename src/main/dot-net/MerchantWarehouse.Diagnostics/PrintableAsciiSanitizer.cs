using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using log4net.Core;
using log4net.Layout;
using log4net.Layout.Pattern;

namespace MerchantWarehouse.Diagnostics
{
    sealed internal class PrintableAsciiSanitizer
    {
        public static string Sanitize(string input, int maxLength, byte[] forbiddenOctets)
        {
            var ascii = System.Text.Encoding.ASCII;

            byte[] asciiBytes = ascii.GetBytes(input);
            System.Collections.Generic.List<byte> sanitizedBytes = new System.Collections.Generic.List<byte>();

            foreach (byte b in asciiBytes)
            {
                if (!(b < 32 || b > 126 || System.Array.FindIndex(forbiddenOctets, item => item == b) != -1))
                {
                    sanitizedBytes.Add(b);
                }
            }

            byte[] sanitizedByteArray = sanitizedBytes.ToArray();
            if (sanitizedByteArray.Length < maxLength)
            {
                maxLength = sanitizedByteArray.Length;
            }

            return ascii.GetString(sanitizedByteArray).Substring(0, maxLength);
        }

        public static string Sanitize(string input, int maxLength)
        {
            return Sanitize(input, maxLength, new byte[] { });
        }
    }
}
