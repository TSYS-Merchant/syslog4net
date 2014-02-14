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

namespace MerchantWarehouse.Diagnostics.Tests
{
    [TestFixture]
    public class ProcessNameConverterTests
    {
        [Test]
        public void ConvertTest()
        {
            var writer = new StreamWriter(new MemoryStream());
            var converter = new CommandLineConverter();

            converter.Format(writer, new LoggingEvent(new LoggingEventData()));
            writer.Flush();

            var result = TestUtilities.GetStringFromStream(writer.BaseStream);

            Assert.AreEqual(Environment.CommandLine, result);

        }
    }
}