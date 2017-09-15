using System;
using System.Text;
using System.IO;
using syslog4net.Layout;
using log4net.Core;
using log4net.Repository;
using NUnit.Framework;
using NSubstitute;

namespace syslog4net.Tests.Layout
{

    using log4net.ObjectRenderer;

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

            var exception = new Exception("test exception message");
            
            // need a non-null ILoggerRepository to prevent NullReferenceException calling layout.Format on a LoggingEvent with an exception.
            ILoggerRepository logRepository = log4net.LogManager
                .GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType).Logger.Repository;  
            
            var evt = new LoggingEvent(typeof(SyslogLayoutTests), logRepository, "test logger", Level.Debug, "test message", exception);

            StringWriter writer = new StringWriter();

            layout.Format(writer, evt);

            string result = writer.ToString();

            // it's hard to test the whole message, because it depends on your machine name, process id, time & date, etc.
            // just test the message's invariant portions
            Assert.IsTrue(result.StartsWith("<135>1 "));
            Assert.IsTrue(result.Contains("[TEST@12345 EventSeverity=\"DEBUG\" ExceptionType=\"System.Exception\" ExceptionMessage=\"test exception message\"]"));
            Assert.IsTrue(result.Contains("test message" + Environment.NewLine));
        }

        [Test]
        public void TestThatStackTraceWritten()
        {
            // we would like a proper stack trace on both exceptions so using a double throw is the closest option to real world usage.
            try
            {
                try
                {
                    throw new Exception("test inner exception");
                }
                catch (Exception ex)
                {
                    // allowing the outer exception to bubble up to higher code block will allow the test to check inner exception is written.
                    throw new Exception("test outer exception", ex);
                }
            }
            catch (Exception ex)
            {
                SyslogLayout layout = new SyslogLayout();
                layout.StructuredDataPrefix = "TEST@12345";
                layout.ActivateOptions();

                // need a non-null ILoggerRepository to prevent NullReferenceException calling layout.Format on a LoggingEvent with an exception.
                ILoggerRepository logRepository = log4net.LogManager
                    .GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType).Logger.Repository;  

                var evt = new LoggingEvent(typeof(SyslogLayoutTests), logRepository, "test logger", Level.Debug, "test message", ex);

                StringWriter writer = new StringWriter();

                layout.Format(writer, evt);

                string result = writer.ToString();
                Assert.IsTrue(result.Contains("System.Exception: test outer exception"));
                Assert.IsTrue(result.Contains("System.Exception: test inner exception"));
                Assert.IsTrue(result.Contains("End of inner exception stack trace"));
            }
        }

        [Test]
        public void TestThatWeTruncateLongMessages()
        {
            SyslogLayout layout = new SyslogLayout();
            layout.StructuredDataPrefix = "TEST@12345";
            layout.ActivateOptions();

            StringBuilder longMessage = new StringBuilder();
            for (int i = 0; i < 2048; i++)
            {
                longMessage.Append("test message");
            }

            var exception = new ArgumentNullException();

            // need a non-null ILoggerRepository to prevent NullReferenceException calling layout.Format on a LoggingEvent with an exception.
            ILoggerRepository logRepository = log4net.LogManager
                .GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType).Logger.Repository;  

            var evt = new LoggingEvent(typeof(SyslogLayoutTests), logRepository, "test logger", Level.Debug, longMessage.ToString(), exception);

            StringWriter writer = new StringWriter();

            layout.Format(writer, evt);

            string result = writer.ToString();

            Assert.AreEqual(2048, result.Length);
        }
        public void TestThatWeTruncateLongMessages5555()
        {
            SyslogLayout layout = new SyslogLayout();
            layout.StructuredDataPrefix = "TEST@12345";
            layout.MaxMessageLength = "5555";
            layout.ActivateOptions();

            StringBuilder longMessage = new StringBuilder();
            for (int i = 0; i < 2048; i++)
            {
                longMessage.Append("test message");
            }

            var exception = new ArgumentNullException();
            
            // need a non-null ILoggerRepository to prevent NullReferenceException calling layout.Format on a LoggingEvent with an exception.
            ILoggerRepository logRepository = log4net.LogManager
                .GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType).Logger.Repository; 
            
            var evt = new LoggingEvent(typeof(SyslogLayoutTests), logRepository, "test logger", Level.Debug, longMessage.ToString(), exception);

            StringWriter writer = new StringWriter();

            layout.Format(writer, evt);

            string result = writer.ToString();

            Assert.AreEqual(5555, result.Length);
        }
    }
}

