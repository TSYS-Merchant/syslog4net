//// Orignal Class Added to Essentials.Diagnostics - 1/14/2014 - Copyright © 2014 Merchant Warehouse
//// All Code Released Under the MS-PL License: http://opensource.org/licenses/MS-PL

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Essential.Diagnostics
{
    /// <summary>
    /// Factory class to configure and create an instance of <see cref="INetworkTextWriter"/> based on the provided data.
    /// </summary>
    public class NetworkTextWriter
    {
        private IPEndPoint _endpoint;
        private Encoding _encoding;

        // TODO: Badly named factory pattern? Crappy Pattern in general? yes. - Review and refactor. 

        /// <summary>
        /// Creates a new instance of the <see cref="NetworkTextWrtier"/>
        /// </summary>
        /// <param name="endpoint">endpoint instance data to use for creating the network connection</param>
        /// <param name="encoding">desired text encoding</param>
        public NetworkTextWriter(IPEndPoint endpoint, Encoding encoding)
        {
            _endpoint = endpoint;
            _encoding = encoding;
        }

        /// <summary>
        /// Creates a new instance of <see cref="INetworkTextWrtier"/> based on provided data
        /// </summary>
        /// <param name="protocol">network protocol to use</param>
        /// <returns><see cref="INetworkTextWriter"/>instance configured for the protocol, encoding and IP information provided.</returns>
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