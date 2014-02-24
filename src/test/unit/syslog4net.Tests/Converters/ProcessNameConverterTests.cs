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

            var expected = TestUtilities.MakePrintableASCII(testRunnerProcess, 48);

            Assert.AreEqual(expected, result);
        }
    }
}