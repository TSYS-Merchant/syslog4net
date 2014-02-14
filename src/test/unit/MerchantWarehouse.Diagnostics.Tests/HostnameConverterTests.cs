using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;

using log4net;
using NUnit.Framework;

using MerchantWarehouse.Diagnostics;
using log4net.Core;
using MerchantWarehouse.Diagnostics.Converters;

namespace MerchantWarehouse.Diagnostics.Tests
{
    [TestFixture]
    public class HostnameConverterTests
    {
        [Test]
        public void ConvertTest()
        {
            var writer = new StreamWriter(new MemoryStream());
            var converter = new HostnameConverter();

            converter.Format(writer, new LoggingEvent(new LoggingEventData()));
            writer.Flush();

            var result = TestUtilities.GetStringFromStream(writer.BaseStream);

            var ipProps = IPGlobalProperties.GetIPGlobalProperties();

            Assert.AreEqual(string.Format("{0}.{1}", ipProps.HostName, ipProps.DomainName).ToUpper(), result);
        }
    }
}