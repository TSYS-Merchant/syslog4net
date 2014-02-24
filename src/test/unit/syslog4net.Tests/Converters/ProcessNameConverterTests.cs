using System.Diagnostics;
using System.IO;
using log4net.Core;
using syslog4net.Converters;
using syslog4net.Util;
using NUnit.Framework;
using System.Text;

namespace syslog4net.Tests.Converters
{
    [TestFixture]
    public class ProcessNameConverterTests
    {
        [Test]
        public void ConvertTest()
        {
            var writer = new StreamWriter(new MemoryStream());
            var converter = new ProcessNameConverter();

            converter.Format(writer, new LoggingEvent(new LoggingEventData()));
            writer.Flush();

            var result = TestUtilities.GetStringFromStream(writer.BaseStream);
            var testRunnerProcess = Process.GetCurrentProcess().ProcessName;

            StringBuilder sb = new StringBuilder();
            foreach (char ch in testRunnerProcess)
            {
                if (ch > 32 && ch < 128)
                {
                    sb.Append(ch);
                }
            }

            Assert.AreEqual(sb.ToString(), result);
        }
    }
}