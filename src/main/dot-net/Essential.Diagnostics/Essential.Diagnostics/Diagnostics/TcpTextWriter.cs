using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Essential.Diagnostics
{
    class TcpTextWriter : INetworkTextWriter
    {
        private object _fileLock = new object();
        private IPEndPoint _endpoint;
        private TcpClient _client = new TcpClient();
        private Encoding _encoding;
        TraceFormatter traceFormatter = new TraceFormatter();

        public TcpTextWriter(IPEndPoint endpoint, Encoding encoding)
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