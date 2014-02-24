using System;
using System.IO;
using System.Text;
using log4net.Core;
using syslog4net.Converters;
using NUnit.Framework;

namespace syslog4net.Tests.Converters
{
    [TestFixture]
    public class CommandLineConverterTests
    {
        [Test]
        public void ConvertTest()
        {
            var writer = new StreamWriter(new MemoryStream());
            var converter = new CommandLineConverter();

            converter.Format(writer, new LoggingEvent(new LoggingEventData()));
            writer.Flush();

            var result = TestUtilities.GetStringFromStream(writer.BaseStream);

            StringBuilder sb = new StringBuilder();
            foreach (char ch in Environment.CommandLine)
            {
                if (ch > 32 && ch < 128)
                {
                    sb.Append(ch);
                }
            }

            Assert.AreEqual(sb.ToString(), result);

        }
    }
}