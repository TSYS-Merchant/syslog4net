using System;
using System.IO;
using syslog4net.Filter;
using log4net.Core;
using log4net.Util;
using log4net.Filter;
using log4net.Repository;
using NUnit.Framework;
using NSubstitute;

namespace syslog4net.Tests.Filter
{
    [TestFixture]
    class LogExceptionToFileFilterTests
    {
        [Test]
        public void ActivateOptionsTestNoLogFolderSet()
        {
            LogExceptionToFileFilter filter = new LogExceptionToFileFilter();
            Assert.That(
                () => filter.ActivateOptions(),
                Throws.Exception
                .TypeOf<ArgumentNullException>()
                .With.Property("ParamName").EqualTo("ExceptionFileLogFolder")
                );            
        }

        [Test]
        public void ActivateOptionsTestLogFolderSet()
        {
            string tempPath = Path.Combine(System.IO.Path.GetTempPath() + "foobar");
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }

            LogExceptionToFileFilter filter = new LogExceptionToFileFilter();
            filter.ExceptionLogFolder = tempPath;

            filter.ActivateOptions();

            var exception = new ArgumentNullException();
            ILoggerRepository logRepository = Substitute.For<ILoggerRepository>();

            var evt = new LoggingEvent(typeof(LogExceptionToFileFilterTests), logRepository, "test logger", Level.Debug, "test message", exception);

            var filterResult = filter.Decide(evt);
            Assert.AreEqual(FilterDecision.Neutral, filterResult);

            Assert.IsTrue(evt.Properties.Contains("log4net:syslog-exception-log"), "has an exception log param");
            Assert.IsTrue(Directory.Exists(tempPath));

            Assert.IsTrue(File.Exists(evt.Properties["log4net:syslog-exception-log"].ToString()), "exception file exists");

            Directory.Delete(tempPath, true);
        }    
    }
}
