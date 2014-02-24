namespace syslog4net.Util
{
    /// <summary>
    /// Provides helper methods for cleaning up string values. It is assumed that the consumer has specific needs for the string. For this implmentation this is mainly used to ensure that syslog
    /// formatted messages are not too long and that they do not include invalid characters in fields like keynames.
    /// </summary>
    internal static class PrintableAsciiSanitizer
    {
        /// <summary>
        /// Cleans up an input string based on the provided data
        /// </summary>
        /// <param name="input">input string to clean</param>
        /// <param name="maxLength">maximum lenght allowed (will trim from the end of the string to the max lenght provided</param>
        /// <param name="forbiddenOctets">character octets to remove from the string</param>
        /// <returns>a cleaned string based upon the provided data</returns>
        public static string Sanitize(string input, int maxLength, byte[] forbiddenOctets)
        {
            var ascii = System.Text.Encoding.ASCII;

            byte[] asciiBytes = ascii.GetBytes(input);
            System.Collections.Generic.List<byte> sanitizedBytes = new System.Collections.Generic.List<byte>();

            foreach (byte b in asciiBytes)
            {
                if (!(b < 33 || b > 126 || System.Array.FindIndex(forbiddenOctets, item => item == b) != -1))
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

        /// <summary>
        /// Cleans up an input string based on the provided data
        /// </summary>
        /// <param name="input">input string to clean</param>
        /// <param name="maxLength">maximum lenght allowed (will trim from the end of the string to the max lenght provided</param>
        /// <returns>a cleaned string based upon the provided data</returns>
        public static string Sanitize(string input, int maxLength)
        {
            return Sanitize(input, maxLength, new byte[] { });
        }
    }
}
