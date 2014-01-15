using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Essential.Diagnostics
{
    public class NetworkTextWriter
    {
        private IPEndPoint _endpoint;
        private Encoding _encoding;

        public NetworkTextWriter(IPEndPoint endpoint, Encoding encoding)
        {
            _endpoint = endpoint;
            _encoding = encoding;
        }

        public INetworkTextWriter Create(ProtocolType protocol)
        {
            switch (protocol)
            {
                case ProtocolType.Udp:
                    return new UdpTextWriter(_endpoint, _encoding);
                case ProtocolType.Tcp:
                    return new TcpTextWriter(_endpoint, _encoding);
            }

            throw new NetworkTextWriterException(string.Format("The provided protocol type of {0} is not supported.", protocol.ToString()));
        }
    }
}