//// Orignal Class Added to Essentials.Diagnostics - 1/14/2014 - Copyright © 2014 Merchant Warehouse
//// All Code Released Under the MS-PL License: http://opensource.org/licenses/MS-PL

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Essential.Diagnostics
{
    /// <summary>
    /// <see cref="INetworkTextWriter"/> implmentation capable of writing strings to a configured TCP port.
    /// </summary>
    class TcpTextWriter : INetworkTextWriter
    {
        private object _fileLock = new object();
        private IPEndPoint _endpoint;
        private TcpClient _client = new TcpClient();
        private Encoding _encoding;
        TraceFormatter traceFormatter = new TraceFormatter();

        /// <summary>
        /// Creates a new instance of <see cref="TcpTextWriter"/> 
        /// </summary>
        /// <param name="endpoint">IP and port information to use when connecting</param>
        /// <param name="encoding">text encoding to use when sending messages</param>
        public TcpTextWriter(IPEndPoint endpoint, Encoding encoding)
        {
            _endpoint = endpoint;
            _encoding = encoding;
        }

        /// <summary>
        /// Writes a message to the configured TCP port
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
        /// Writes a message with an appended carriage return to the configured TCP port
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
                if (!_client.Connected)
                {
                    _client = new TcpClient();
                    _client.Connect(_endpoint);
                }

                var stream = _client.GetStream();

                using (var writer = new StreamWriter(stream, _encoding))
                {
                    writer.Write(message);
                    //writer.Write("<132>1 2013-10-28T00:05:22:Z SERENITY HelloWorker 7260-11 SOMENAME [MW@4500 EventClass=\"SOMEEVENTCLASS\" EventSeverity=\"Warning\" EventHelp=\"http://merchantwarehouse.com\"] Worker Worker 1 getting annoyed");

                    /*
                    using (BinaryReader reader = new BinaryReader(stream, _encoding))
                    {
                        var resp = reader.ReadString();
                    }*/
                }
                //_client.Close();
            }
            catch (Exception ex)
            {
                var ex2 = ex;
            }
        }
    }
}