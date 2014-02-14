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
using MerchantWarehouse.Diagnostics.Tests;
using NUnit.Framework;

using MerchantWarehouse.Diagnostics;
using MerchantWarehouse.Diagnostics.Converters;


namespace MerchantWarehouse.Diagnostics.Tests
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
            var testId = "9001";
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