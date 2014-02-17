using System.Collections.Generic;
using System.IO;
using log4net.Core;
using MerchantWarehouse.Diagnostics.Converters;
using NUnit.Framework;

namespace MerchantWarehouse.Diagnostics.Tests.Converters
{
    [TestFixture]
    public class PriorityConverterTests
    {
        [Test]
        public void ConvertTest()
        {
            var expectedData = new Dictionary<Level, string>()
            {
                { Level.Emergency, "128" },
                { Level.Fatal, "130" },
                { Level.Error, "131" },
                { Level.Warn, "132" },
                { Level.Info, "134" },
                { Level.Debug, "135" }
            };

            foreach (var item in expectedData)
            {
                var level = item.Key;
                var code = item.Value;

                var writer = new StreamWriter(new MemoryStream());
                var converter = new PriorityConverter();

                converter.Format(writer, new LoggingEvent(new LoggingEventData() { Level = level }));
                writer.Flush();

                var result = TestUtilities.GetStringFromStream(writer.BaseStream);

                Assert.AreEqual(code, result);
            }
        }
    }
}