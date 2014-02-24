using System;
using System.IO;
using syslog4net.Layout;
using log4net.Core;
using log4net.Util;
using log4net.Filter;
using log4net.Repository;
using NUnit.Framework;
using NSubstitute;

namespace syslog4net.Tests.Layout
{
    [TestFixture]
    class SyslogLayoutTests
    {
        [Test]
        public void ActivateOptionsTestNoStructuredDataPrefixSet()
        {
            SyslogLayout layout = new SyslogLayout();
            Assert.That(
                () => layout.ActivateOptions(),
                Throws.Exception
                .TypeOf<ArgumentNullException>()
                .With.Property("ParamName").EqualTo("StructuredDataPrefix")
                );
        }

        [Test]
        public void TestFormat()
        {
            SyslogLayout layout = new SyslogLayout();
            layout.StructuredDataPrefix = "TEST@12345";
            layout.ActivateOptions();

            var exception = new ArgumentNullException();
            ILoggerRepository logRepository = Substitute.For<ILoggerRepository>();
            var evt = new LoggingEvent(typeof(SyslogLayoutTests), logRepository, "test logger", Level.Debug, "test message", exception);

            StringWriter writer = new StringWriter();

            layout.Format(writer, evt);

            string result = writer.ToString();

            // it's hard to test the whole message, because it depends on your machine name, process id, time & date, etc.
            // just test the message's invariant portions
            Assert.IsTrue(result.StartsWith("<135>1 "));
            Assert.IsTrue(result.Contains("[TEST@12345 EventSeverity=\"DEBUG\" ExceptionType=\"System.ArgumentNullException\" ExceptionMessage=\"Value cannot be null.\"]"));
            Assert.IsTrue(result.EndsWith("test message" + Environment.NewLine));
        }
    }
}
