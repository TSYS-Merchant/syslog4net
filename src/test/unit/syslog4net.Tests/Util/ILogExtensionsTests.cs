#if NET_3_5 || NET_4_0

using System.IO;
using log4net;
using log4net.Core;
using log4net.Util;
using syslog4net.Converters;
using NSubstitute;
using NUnit.Framework;
using syslog4net.Util;
using System;

namespace syslog4net.Tests.Util
{
    [TestFixture]
    class ILogExtensionsTests
    {
        [Test]
        public void TestThatStartThreadLogicalActivitySetsTheMessageId()
        {
            var testId = "LogicalThreadActivity";
            var writer = new StreamWriter(new MemoryStream());
            var converter = new MessageIdConverter();

            var log = Substitute.For<ILog>();
            using (log.StartThreadLogicialActivity("NDC", testId))
            {

                converter.Format(writer, new LoggingEvent(new LoggingEventData()));
                writer.Flush();

                var result = TestUtilities.GetStringFromStream(writer.BaseStream);

                Assert.AreEqual(testId, result);
            }
        }
    }
}

#endif