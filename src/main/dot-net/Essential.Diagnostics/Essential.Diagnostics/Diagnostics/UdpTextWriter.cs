//// Orignal Class Added to Essentials.Diagnostics - 1/14/2014 - Copyright © 2014 Merchant Warehouse
//// All Code Released Under the MS-PL License: http://opensource.org/licenses/MS-PL

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Essential.Diagnostics
{
    /// <summary>
    /// <see cref="INetworkTextWriter"/> implmentation capable of writing strings to a configured UDP port.
    /// </summary>
    class UdpTextWriter : INetworkTextWriter
    {
        private object _fileLock = new object();
        private IPEndPoint _endpoint;
        private UdpClient _client = new UdpClient();
        private Encoding _encoding;
        TraceFormatter traceFormatter = new TraceFormatter();

        /// <summary>
        /// Creates a new instance of <see cref="UdpTextWriter"/> 
        /// </summary>
        /// <param name="endpoint">IP and port information to use when connecting</param>
        /// <param name="encoding">text encoding to use when sending messages</param>
        public UdpTextWriter(IPEndPoint endpoint, Encoding encoding)
        {
            _endpoint = endpoint;
            _encoding = encoding;
        }

        /// <summary>
        /// Writes a message to the configured UDP port
        /// </summary>
        /// <param name="eventCache">logging event context data</param>
        /// <param name="value">value to output to the file</param>
        public void Write(TraceEventCache eventCache, string value)
        {
            lock (_fileLock)
            {
                SendMessage(value);
            }
        }

        /// <summary>
        /// Writes a message with an appended carriage return to the configured UDP port
        /// </summary>
        /// <param name="eventCache">logging event context data</param>
        /// <param name="value">value to output to the file</param>
        public void WriteLine(TraceEventCache eventCache, string value)
        {
            lock (_fileLock)
            {
                SendMessage(value);
            }
        }

        /// <summary>
        /// Disposes of the instance and all interal Disposable types
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_client != null)
                {
                    _client.Close();
                }
            }
        }

        private void SendMessage(string message)
        {
            try
            {
                var buffer = _encoding.GetBytes(message);
                _client.Send(buffer, buffer.Length, _endpoint);
            }
            catch (Exception ex)
            {
                // TODO: Review exception types for UdpClient
                //var ex2 = ex;
            }
        }
    }
}
