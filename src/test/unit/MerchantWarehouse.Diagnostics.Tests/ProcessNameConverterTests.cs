using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using log4net;
using NUnit.Framework;

using MerchantWarehouse.Diagnostics;
using log4net.Core;
using MerchantWarehouse.Diagnostics.Converters;
using System.Diagnostics;

namespace MerchantWarehouse.Diagnostics.Tests
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