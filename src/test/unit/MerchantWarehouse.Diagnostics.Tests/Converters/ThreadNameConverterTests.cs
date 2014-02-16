using System.IO;
using System.Threading;
using log4net.Core;
using MerchantWarehouse.Diagnostics.Converters;
using NUnit.Framework;

namespace MerchantWarehouse.Diagnostics.Tests.Converters
{
    [TestFixture]
    public class ThreadNameConverterTests
    {
        [Test]
        public void ConvertTest()
        {
            var writer = new StreamWriter(new MemoryStream());
            var converter = new ThreadNameConverter();

            converter.Format(writer, new LoggingEvent(new LoggingEventData()));
            writer.Flush();

            var result = TestUtilities.GetStringFromStream(writer.BaseStream);

            Assert.AreEqual(Thread.CurrentThread.Name, result);

        }
    }
}