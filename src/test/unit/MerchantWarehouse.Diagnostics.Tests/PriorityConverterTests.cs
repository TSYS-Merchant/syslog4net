using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using log4net;
using log4net.Core;
using log4net.Util;

using NUnit.Framework;

using MerchantWarehouse.Diagnostics;

namespace MerchantWarehouse.Diagnostics.Tests
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