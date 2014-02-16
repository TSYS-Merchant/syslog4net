using System.IO;
using log4net;
using log4net.Core;
using log4net.Util;
using MerchantWarehouse.Diagnostics.Converters;
using NSubstitute;
using NUnit.Framework;

namespace MerchantWarehouse.Diagnostics.Tests.Converters
{
    [TestFixture]
    public class MessageIdConverterTests
    {
        [Test]
        public void ConvertTestLoggingEventData()
        {
            var testId = "9001";
            var writer = new StreamWriter(new MemoryStream());
            var converter = new MessageIdConverter();
            var props = new PropertiesDictionary();
            props["MessageId"] = testId;

            converter.Format(writer, new LoggingEvent(new LoggingEventData() { Properties = props }));
            writer.Flush();

            var result = TestUtilities.GetStringFromStream(writer.BaseStream);

            Assert.AreEqual(testId, result);
        }

        [Test]
        public void ConvertTestStackData()
        {
            var testId = "9001";
            var writer = new StreamWriter(new MemoryStream());
            var converter = new MessageIdConverter();

            var log = Substitute.For<ILog>();
            log.StartMessage(testId);

            converter.Format(writer, new LoggingEvent(new LoggingEventData()));
            writer.Flush();

            var result = TestUtilities.GetStringFromStream(writer.BaseStream);

            Assert.AreEqual(testId, result);
        }

        [Test]
        public void ConvertTestLoggingEventOverridesStackData()
        {
            var propTestId = "9001";
            var ndcTestId = "0";

            var writer = new StreamWriter(new MemoryStream());
            var converter = new MessageIdConverter();
            var props = new PropertiesDictionary();
            props["MessageId"] = propTestId;

            var log = Substitute.For<ILog>();
            log.StartMessage(ndcTestId);

            converter.Format(writer, new LoggingEvent(new LoggingEventData() { Properties = props }));
            writer.Flush();

            var result = TestUtilities.GetStringFromStream(writer.BaseStream);

            Assert.AreEqual(propTestId, result);
        }
    }
}