using System.Diagnostics;
using System.IO;
using log4net.Core;
using log4net.Util;
using syslog4net.Converters;
using NUnit.Framework;

namespace syslog4net.Tests.Converters
{
    [TestFixture]
    public class ProcessIdConverterTests
    {
        [Test]
        public void ConvertTest()
        {
            var writer = new StreamWriter(new MemoryStream());
            var converter = new ProcessIdConverter();

            converter.Format(writer, new LoggingEvent(new LoggingEventData()));
            writer.Flush();

            var result = TestUtilities.GetStringFromStream(writer.BaseStream);

            Assert.AreEqual(Process.GetCurrentProcess().Id.ToString(), result);
        }

        [Test]
        public void ConvertTestWithLoggingEventProperty()
        {
            var testId = Process.GetCurrentProcess().Id.ToString();
            var writer = new StreamWriter(new MemoryStream());
            var converter = new ProcessIdConverter();
            var props = new PropertiesDictionary();
            props["ProcessId"] = testId;

            converter.Format(writer, new LoggingEvent(new LoggingEventData() { Properties = props } ));
            writer.Flush();

            var result = TestUtilities.GetStringFromStream(writer.BaseStream);

            Assert.AreEqual(testId, result);
        }
    }
}