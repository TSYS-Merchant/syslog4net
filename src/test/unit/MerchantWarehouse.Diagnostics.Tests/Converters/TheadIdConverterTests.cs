using System.IO;
using log4net.Core;
using syslog4net.Converters;
using NUnit.Framework;

namespace syslog4net.Tests.Converters
{
    [TestFixture]
    public class ThreadIdConverterTests
    {
        [Test]
        public void ConvertTest()
        {
            var writer = new StreamWriter(new MemoryStream());
            var converter = new ThreadIdConverter();

            converter.Format(writer, new LoggingEvent(new LoggingEventData()));
            writer.Flush();

            var result = TestUtilities.GetStringFromStream(writer.BaseStream);

            Assert.AreEqual(System.Threading.Thread.CurrentThread.ManagedThreadId.ToString(), result);

        }
    }
}