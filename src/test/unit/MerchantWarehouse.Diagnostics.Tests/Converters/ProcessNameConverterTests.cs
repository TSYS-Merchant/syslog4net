using System.Diagnostics;
using System.IO;
using log4net.Core;
using MerchantWarehouse.Diagnostics.Converters;
using NUnit.Framework;

namespace MerchantWarehouse.Diagnostics.Tests.Converters
{
    [TestFixture]
    public class CommandLineConverterTests
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

            Assert.AreEqual(testRunnerProcess, result);
        }
    }
}