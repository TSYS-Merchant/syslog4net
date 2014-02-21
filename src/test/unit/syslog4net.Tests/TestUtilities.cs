using System.IO;

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
    }
}
