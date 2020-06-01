#region Apache License
//
// Licensed to the Apache Software Foundation (ASF) under one or more 
// contributor license agreements. See the NOTICE file distributed with
// this work for additional information regarding copyright ownership. 
// The ASF licenses this file to you under the Apache License, Version 2.0
// (the "License"); you may not use this file except in compliance with 
// the License. You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using log4net.Appender;
using log4net.Core;

namespace syslog4net.Appender
{
    /// <summary>
    /// Sends logging events through TCP connection to a remote host
    /// </summary>
    /// <remarks>
    /// <para>
    /// TCP guarantees delivery and order
    /// </para>
    /// <para>
    /// To view the logging results, a custom application can be developed that listens for logging 
    /// events.
    /// </para>
    /// <para>
    /// When decoding events send via this appender remember to use the same encoding
    /// to decode the events as was used to send the events. See the <see cref="Encoding"/>
    /// property to specify the encoding to use.
    /// </para>
    /// </remarks>
    /// <example>
    /// This example shows how to log receive logging events that are sent 
    /// on IP address 244.0.0.1 and port 8080 to the console. The event is 
    /// encoded in the packet as a unicode string and it is decoded as such. 
    /// <code lang="C#">
    /// IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
    /// TcpClient TcpClient;
    /// byte[] buffer;
    /// string loggingEvent;
    /// 
    /// try 
    /// {
    ///     TcpClient = new TcpClient(8080);
    ///     
    ///     while(true) 
    ///     {
    ///         buffer = TcpClient.Receive(ref remoteEndPoint);
    ///         loggingEvent = System.Text.Encoding.Unicode.GetString(buffer);
    ///         Console.WriteLine(loggingEvent);
    ///     }
    /// } 
    /// catch(Exception e) 
    /// {
    ///     Console.WriteLine(e.ToString());
    /// }
    /// </code>
    /// <code lang="Visual Basic">
    /// Dim remoteEndPoint as IPEndPoint
    /// Dim TcpClient as TcpClient
    /// Dim buffer as Byte()
    /// Dim loggingEvent as String
    /// 
    /// Try 
    ///     remoteEndPoint = new IPEndPoint(IPAddress.Any, 0)
    ///     TcpClient = new TcpClient(8080)
    ///
    ///     While True
    ///         buffer = TcpClient.Receive(ByRef remoteEndPoint)
    ///         loggingEvent = System.Text.Encoding.Unicode.GetString(buffer)
    ///         Console.WriteLine(loggingEvent)
    ///     Wend
    /// Catch e As Exception
    ///     Console.WriteLine(e.ToString())
    /// End Try
    /// </code>
    /// <para>
    /// An example configuration section to log information using this appender to the 
    /// IP 224.0.0.1 on port 8080:
    /// </para>
    /// <code lang="XML" escaped="true">
    /// <appender name="TcpAppender" type="log4net.Appender.TcpAppender">
    ///     <remoteAddress value="224.0.0.1" />
    ///     <remotePort value="8080" />
    ///     <layout type="log4net.Layout.PatternLayout" value="%-5level %logger [%ndc] - %message%newline" />
    /// </appender>
    /// </code>
    /// </example>
    /// <author>Gert Driesen</author>
    /// <author>Nicko Cadell</author>
    public class TcpAppender : AppenderSkeleton
    {
        private static int _failedConnectionCount = 0;

        #region Public Instance Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpAppender" /> class.
        /// </summary>
        /// <remarks>
        /// The default constructor initializes all fields to their default values.
        /// </remarks>
        public TcpAppender()
        {
            // set port to some invalid value, forcing you to set the port
            this._remotePort = IPEndPoint.MinPort - 1;
        }

        private class AsyncLoggingData
        {
            internal TcpClient Client { get; set; }
            internal LoggingEvent LoggingEvent { get; set; }
            internal string Message { get; set; }
        }

        #endregion Public Instance Constructors

        #region Public Instance Properties

        /// <summary>
        /// Gets or sets the IP address of the remote host to which
        /// the underlying <see cref="TcpClient" /> should sent the logging event.
        /// </summary>
        /// <value>
        /// The IP address of the remote host  to which the logging event will be sent.
        /// </value>
        /// <remarks>
        /// <para>
        /// Multicast addresses are identified by IP class <b>D</b> addresses (in the range 224.0.0.0 to
        /// 239.255.255.255).  Multicast packets can pass across different networks through routers, so
        /// it is possible to use multicasts in an Internet scenario as long as your network provider 
        /// supports multicasting.
        /// </para>
        /// <para>
        /// Hosts that want to receive particular multicast messages must register their interest by joining
        /// the multicast group.  Multicast messages are not sent to networks where no host has joined
        /// the multicast group.  Class <b>D</b> IP addresses are used for multicast groups, to differentiate
        /// them from normal host addresses, allowing nodes to easily detect if a message is of interest.
        /// </para>
        /// <para>
        /// Static multicast addresses that are needed globally are assigned by IANA.  A few examples are listed in the table below:
        /// </para>
        /// <para>
        /// <list type="table">
        ///     <listheader>
        ///         <term>IP Address</term>
        ///         <description>Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term>224.0.0.1</term>
        ///         <description>
        ///             <para>
        ///             Sends a message to all system on the subnet.
        ///             </para>
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>224.0.0.2</term>
        ///         <description>
        ///             <para>
        ///             Sends a message to all routers on the subnet.
        ///             </para>
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>224.0.0.12</term>
        ///         <description>
        ///             <para>
        ///             The DHCP server answers messages on the IP address 224.0.0.12, but only on a subnet.
        ///             </para>
        ///         </description>
        ///     </item>
        /// </list>
        /// </para>
        /// <para>
        /// A complete list of actually reserved multicast addresses and their owners in the ranges
        /// defined by RFC 3171 can be found at the <A href="http://www.iana.org/assignments/multicast-addresses">IANA web site</A>. 
        /// </para>
        /// <para>
        /// The address range 239.0.0.0 to 239.255.255.255 is reserved for administrative scope-relative 
        /// addresses.  These addresses can be reused with other local groups.  Routers are typically 
        /// configured with filters to prevent multicast traffic in this range from flowing outside
        /// of the local network.
        /// </para>
        /// </remarks>
        public IPAddress RemoteAddress
        {
            get { return this._remoteAddress; }
            set { this._remoteAddress = value; }
        }

        /// <summary>
        /// Gets or sets the TCP port number of the remote host or multicast group to which 
        /// the underlying <see cref="TcpClient" /> should sent the logging event.
        /// </summary>
        /// <value>
        /// An integer value in the range <see cref="IPEndPoint.MinPort" /> to <see cref="IPEndPoint.MaxPort" /> 
        /// indicating the TCP port number of the remote host or multicast group to which the logging event 
        /// will be sent.
        /// </value>
        /// <remarks>
        /// The underlying <see cref="TcpClient" /> will send messages to this TCP port number
        /// on the remote host or multicast group.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">The value specified is less than <see cref="IPEndPoint.MinPort" /> or greater than <see cref="IPEndPoint.MaxPort" />.</exception>
        public int RemotePort
        {
            get { return this._remotePort; }
            set
            {
                if (value < IPEndPoint.MinPort || value > IPEndPoint.MaxPort)
                {
                    throw log4net.Util.SystemInfo.CreateArgumentOutOfRangeException("value", value,
                        "The value specified is less than " +
                        IPEndPoint.MinPort.ToString(NumberFormatInfo.InvariantInfo) +
                        " or greater than " +
                        IPEndPoint.MaxPort.ToString(NumberFormatInfo.InvariantInfo) + ".");
                }
                else
                {
                    this._remotePort = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets <see cref="Encoding"/> used to write the packets.
        /// </summary>
        /// <value>
        /// The <see cref="Encoding"/> used to write the packets.
        /// </value>
        /// <remarks>
        /// <para>
        /// The <see cref="Encoding"/> used to write the packets.
        /// </para>
        /// </remarks>
        public Encoding Encoding
        {
            get { return this._encoding; }
            set { this._encoding = value; }
        }

        #endregion Public Instance Properties

        #region Implementation of IOptionHandler

        /// <summary>
        /// Initialize the appender based on the options set.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is part of the <see cref="IOptionHandler"/> delayed object
        /// activation scheme. The <see cref="ActivateOptions"/> method must 
        /// be called on this object after the configuration properties have
        /// been set. Until <see cref="ActivateOptions"/> is called this
        /// object is in an undefined state and must not be used. 
        /// </para>
        /// <para>
        /// If any of the configuration properties are modified then 
        /// <see cref="ActivateOptions"/> must be called again.
        /// </para>
        /// <para>
        /// The appender will be ignored if no <see cref="RemoteAddress" /> was specified or 
        /// an invalid remote or local TCP port number was specified.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">The required property <see cref="RemoteAddress" /> was not specified.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The TCP port number assigned to <see cref="RemotePort" /> is less than <see cref="IPEndPoint.MinPort" /> or greater than <see cref="IPEndPoint.MaxPort" />.</exception>
        public override void ActivateOptions()
        {
            base.ActivateOptions();

            if (this.RemoteAddress == null)
            {
                throw new ArgumentNullException("RemoteAddress");
            }
            
            if (this.RemotePort < IPEndPoint.MinPort || this.RemotePort > IPEndPoint.MaxPort)
            {
                throw log4net.Util.SystemInfo.CreateArgumentOutOfRangeException("RemotePort", this.RemotePort,
                    "The RemotePort is less than " +
                    IPEndPoint.MinPort.ToString(NumberFormatInfo.InvariantInfo) +
                    " or greater than " +
                    IPEndPoint.MaxPort.ToString(NumberFormatInfo.InvariantInfo) + ".");
            }
        }

        #endregion

        #region Override implementation of AppenderSkeleton

        /// <summary>
        /// This method is called by the <see cref="M:AppenderSkeleton.DoAppend(LoggingEvent)"/> method.
        /// </summary>
        /// <param name="loggingEvent">The event to log.</param>
        /// <remarks>
        /// <para>
        /// Sends the event using an Tcp datagram.
        /// </para>
        /// <para>
        /// Exceptions are passed to the <see cref="AppenderSkeleton.ErrorHandler"/>.
        /// </para>
        /// </remarks>
        protected override void Append(LoggingEvent loggingEvent)
        {

            try
            {
                TcpClient client = new TcpClient();

                string message = RenderLoggingEvent(loggingEvent);

                //Async Programming Model allows socket connection to happen on threadpool so app can continue.
                client.BeginConnect(
                    this.RemoteAddress,
                    this.RemotePort,
                    this.ConnectionEstablishedCallback,
                    new AsyncLoggingData()
                    {
                        Client = client,
                        LoggingEvent = loggingEvent,
                        Message = message
                    });
            }
            catch (Exception ex)
            {
                ErrorHandler.Error(
                    "Unable to send logging event to remote host " + this.RemoteAddress.ToString() + " on port " +
                    this.RemotePort + ".", ex, ErrorCode.WriteFailure);
            }
        }

        private void ConnectionEstablishedCallback(IAsyncResult asyncResult)
        {
            // TODO callback happens on background thread. lose data if app pool recycled?
            AsyncLoggingData loggingData = asyncResult.AsyncState as AsyncLoggingData;
            if (loggingData == null)
            {
                throw new ArgumentException("LoggingData is null", "loggingData");
            }

            try
            {
                loggingData.Client.EndConnect(asyncResult);
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref _failedConnectionCount);
                if (_failedConnectionCount >= 1)
                {
                    //We have failed to connect to all the IP Addresses. connection has failed overall.
                    ErrorHandler.Error(
                        "Unable to send logging event to remote host " + this.RemoteAddress.ToString() + " on port " +
                        this.RemotePort + ".", ex, ErrorCode.FileOpenFailure);
                    return;
                }
            }

            try
            {
                Byte[] buffer = this._encoding.GetBytes(loggingData.Message.ToCharArray());
                using (var netStream = loggingData.Client.GetStream())
                {
                    netStream.Write(buffer, 0, buffer.Length);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Error(
                    "Unable to send logging event to remote host " + this.RemoteAddress.ToString() + " on port " +
                    this.RemotePort + ".", ex, ErrorCode.WriteFailure);
            }
            finally
            {
                loggingData.Client.Close();
            }
        }

        /// <summary>
        /// This appender requires a <see cref="log4net.Layout"/> to be set.
        /// </summary>
        /// <value><c>true</c></value>
        /// <remarks>
        /// <para>
        /// This appender requires a <see cref="log4net.Layout"/> to be set.
        /// </para>
        /// </remarks>
        override protected bool RequiresLayout
        {
            get { return true; }
        }

        /// <summary>
        /// Closes the Tcp connection and releases all resources associated with 
        /// this <see cref="TcpAppender" /> instance.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Disables the underlying <see cref="TcpClient" /> and releases all managed 
        /// and unmanaged resources associated with the <see cref="TcpAppender" />.
        /// </para>
        /// </remarks>
        override protected void OnClose()
        {
            base.OnClose();
        }

        #endregion Override implementation of AppenderSkeleton

        #region Private Instance Fields

        /// <summary>
        /// The IP address of the remote host or multicast group to which 
        /// the logging event will be sent.
        /// </summary>
        private IPAddress _remoteAddress;

        /// <summary>
        /// The TCP port number of the remote host or multicast group to 
        /// which the logging event will be sent.
        /// </summary>
        private int _remotePort;

        /// <summary>
        /// The encoding to use for the packet.
        /// </summary>
        private Encoding _encoding = Encoding.UTF8;

        #endregion Private Instance Fields
    }
}
