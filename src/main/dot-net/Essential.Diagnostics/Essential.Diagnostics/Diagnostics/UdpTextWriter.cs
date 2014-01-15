using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Essential.Diagnostics
{
    class UdpTextWriter : INetworkTextWriter
    {
        private object _fileLock = new object();
        private IPEndPoint _endpoint;
        private UdpClient _client = new UdpClient();
        private Encoding _encoding;
        TraceFormatter traceFormatter = new TraceFormatter();

        public UdpTextWriter(IPEndPoint endpoint, Encoding encoding)
        {
            _endpoint = endpoint;
            _encoding = encoding;
        }

        public void Write(TraceEventCache eventCache, string value)
        {
            lock (_fileLock)
            {
                SendMessage(value);
            }
        }

        public void WriteLine(TraceEventCache eventCache, string value)
        {
            lock (_fileLock)
            {
                SendMessage(value);
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
                var ex2 = ex;
            }
        }

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

    }
}
