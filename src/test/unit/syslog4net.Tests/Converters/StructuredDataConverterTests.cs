using System;
using System.IO;
using log4net.Core;
using log4net.Util;
using log4net.Repository;
using syslog4net.Converters;
using NUnit.Framework;
using NSubstitute;

namespace syslog4net.Tests.Converters
{
    [TestFixture]
    public class StructuredDataConverterTests
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

        [Test]
        public void ConvertTestWithExceptionString()
        {
            var level = Level.Debug;
            var testId = "9001";
            var exceptionMessage = "exception occurred";
            var resultString = "[MW@55555 MessageId=\"" + testId + "\" EventSeverity=\"" + level.DisplayName + "\" ExceptionMessage=\"" + exceptionMessage + "\"]";
            var writer = new StreamWriter(new MemoryStream());
            var converter = new StructuredDataConverter();
            var props = new PropertiesDictionary();
            props["MessageId"] = testId;
            props["log4net:StructuredDataPrefix"] = "MW@55555";

            var evt = new LoggingEvent(new LoggingEventData() { Properties = props, Level = level, ExceptionString = exceptionMessage });

            converter.Format(writer, evt);

            writer.Flush();

            var result = TestUtilities.GetStringFromStream(writer.BaseStream);

            Assert.AreEqual(resultString, result);
        }

        [Test]
        public void ConvertTestWithExceptionObject()
        {
            var level = Level.Debug;
            var writer = new StreamWriter(new MemoryStream());
            var converter = new StructuredDataConverter();

            var exception = new ArgumentNullException();
            ILoggerRepository logRepository = Substitute.For<ILoggerRepository>();

            var evt = new LoggingEvent(typeof(StructuredDataConverterTests), logRepository, "test logger", level, "test message", exception);

            evt.Properties["log4net:StructuredDataPrefix"] = "TEST@12345";

            converter.Format(writer, evt);

            writer.Flush();

            var result = TestUtilities.GetStringFromStream(writer.BaseStream);

            Assert.AreEqual("[TEST@12345 EventSeverity=\"DEBUG\" ExceptionType=\"System.ArgumentNullException\" ExceptionMessage=\"Value cannot be null.\"]", result);
        }    
    }
}