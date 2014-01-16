using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Essential.Diagnostics.Tests.Utility;
using NUnit.Framework; 

namespace Essential.Diagnostics.Tests
{
    [TestFixture]
    public class RollingFileTests
    {
        public TestContext TestContext { get; set; }

        [Test]
        public void FileHandlesEventSentDirectly()
        {
            var mockFileSystem = new MockFileSystem();
            var listener = new RollingFileTraceListener(null);
            listener.FileSystem = mockFileSystem;

            listener.TraceEvent(null, "source", TraceEventType.Information, 1, "{0}-{1}", 2, "A");
            listener.Flush();

            Assert.AreEqual(1, mockFileSystem.OpenedItems.Count);
            var tuple0 = mockFileSystem.OpenedItems[0];
            // VS2012 process name (earlier name was "QTAgent32-")
            StringAssert.StartsWith("vstest.executionengine.x86-" + DateTimeOffset.Now.Year.ToString(), tuple0.Item1);
            var data = tuple0.Item2.GetBuffer();
            var output = Encoding.UTF8.GetString(data, 0, (int)tuple0.Item2.Length);
            StringAssert.Contains("Information source 1: 2-A", output);
        }

        [Test]
        public void FileHandlesEventFromTraceSource()
        {
            var mockFileSystem = new MockFileSystem();
            TraceSource source = new TraceSource("rollingFile1Source");
            var listener = source.Listeners.OfType<RollingFileTraceListener>().First();
            listener.FileSystem = mockFileSystem;

            source.TraceEvent(TraceEventType.Warning, 2, "{0}-{1}", 3, "B");
            source.Flush(); // or have AutoFlush configured

            Assert.AreEqual(1, mockFileSystem.OpenedItems.Count);
            var tuple0 = mockFileSystem.OpenedItems[0];
            // VS2012 process name (earlier name was "QTAgent32-")
            StringAssert.StartsWith("vstest.executionengine.x86-" + DateTimeOffset.Now.Year.ToString(), tuple0.Item1);
            var output = Encoding.UTF8.GetString(tuple0.Item2.GetBuffer(), 0, (int)tuple0.Item2.Length);
            StringAssert.Contains("Warning rollingFile1Source 2: 3-B", output);
        }

        [Test]
        public void FileRollOverTest()
        {
            var mockFileSystem = new MockFileSystem();
            var listener = new RollingFileTraceListener("Log{DateTime:HHmmss}");
            listener.FileSystem = mockFileSystem;

            listener.TraceEvent(null, "souce", TraceEventType.Information, 1, "A");
            Thread.Sleep(TimeSpan.FromSeconds(2));
            listener.TraceEvent(null, "souce", TraceEventType.Information, 2, "B");
            listener.Flush();

            Assert.AreEqual(2, mockFileSystem.OpenedItems.Count);
        }

        [Test]
        public void FileConfigParametersLoadedCorrectly()
        {
            TraceSource source = new TraceSource("rollingFile2Source");
            var listener = source.Listeners.OfType<RollingFileTraceListener>().First();

            Assert.AreEqual("rollingFile2", listener.Name);
            Assert.AreEqual("{DateTime},{EventType},{Message}", listener.Template);
            Assert.AreEqual("Trace{DateTime:yyyyMMdd}.log", listener.FilePathTemplate);
        }

    }
}
