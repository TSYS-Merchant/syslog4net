using System.IO;
using log4net.Core;
using log4net.Util;
using syslog4net.Converters;
using NUnit.Framework;

namespace syslog4net.Tests.Converters
{
    [TestFixture]
    public class StrcturedDataConverterTests
    {
        [Test]
        public void ConvertTestNoException()
        {
            var level = Level.Debug;
            var testId = "9001";
            var resultString = "[MW@55555 MessageId=\"" + testId + "\" EventSeverity=\"" + level.DisplayName + "\"]";
            var writer = new StreamWriter(new MemoryStream());
            var converter = new StructuredDataConverter();
            var props = new PropertiesDictionary();
            props["MessageId"] = testId;
            props["log4net:StructuredDataPrefix"] = "MW@55555";

            // Additional tests using stack data is prevented as the converter class only pulls from LoggingEvent.GetProperties()
            // The data behind this method is setup by log4net itself so testing stack usage would not really apply properly here
            //ThreadContext.Stacks["foo"].Push("bar");

            var evt = new LoggingEvent(new LoggingEventData() { Properties = props, Level = level });

            converter.Format(writer, evt);
            
            writer.Flush();

            var result = TestUtilities.GetStringFromStream(writer.BaseStream);

            Assert.AreEqual(resultString, result);
        }
    }
}