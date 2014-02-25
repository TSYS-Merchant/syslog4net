using log4net;
using log4net.Core;
using log4net.Util;
using syslog4net.Appender;
using NSubstitute;
using NUnit.Framework;
using syslog4net.Util;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace syslog4net.Tests.Appender
{
    [TestFixture]
    class TcpAppenderTests
    {
        [Test]
        public void EncodingTests()
        {
            TcpAppender appender = new TcpAppender();

            appender.Encoding = Encoding.UTF32;

            Assert.AreEqual(Encoding.UTF32, appender.Encoding);
        }

        [Test]
        public void RemoteAddressTests()
        {
            TcpAppender appender = new TcpAppender();

            IPAddress address = IPAddress.Parse("127.0.0.1");

            appender.RemoteAddress = address;

            Assert.AreEqual(address, appender.RemoteAddress);
        }

        [Test]
        public void TestRemotePort()
        {
            TcpAppender appender = new TcpAppender();

            // negative tests
            Assert.That(
                () => appender.RemotePort = IPEndPoint.MinPort - 1,
                Throws.Exception
                .TypeOf<ArgumentOutOfRangeException>()
                .With.Property("ParamName").EqualTo("value")
                );

            Assert.That(
                () => appender.RemotePort = IPEndPoint.MaxPort + 1,
                Throws.Exception
                .TypeOf<ArgumentOutOfRangeException>()
                .With.Property("ParamName").EqualTo("value")
                );

            // positive test
            int goodPort = IPEndPoint.MinPort + 1;
            appender.RemotePort = goodPort;

            Assert.AreEqual(goodPort, appender.RemotePort);
        }

        [Test]
        public void TestActivateOptions()
        {
            TcpAppender appender = new TcpAppender();

            // negative test - neither the remote port or address has been configured
            Assert.That(
                () => appender.ActivateOptions(),
                Throws.Exception
                .TypeOf<ArgumentNullException>()
                .With.Property("ParamName").EqualTo("Address")
                );

            // ok, set the remote address
            appender.RemoteAddress = IPAddress.Parse("127.0.0.1");

            // negative test - no remote port set
            Assert.That(
                () => appender.ActivateOptions(),
                Throws.Exception
                .TypeOf<ArgumentOutOfRangeException>()
                .With.Property("ParamName").EqualTo("RemotePort")
                );

            // ok, set a valid port
            appender.RemotePort = IPEndPoint.MinPort + 1;

            // should pass
            appender.ActivateOptions();
        }
    }
}
