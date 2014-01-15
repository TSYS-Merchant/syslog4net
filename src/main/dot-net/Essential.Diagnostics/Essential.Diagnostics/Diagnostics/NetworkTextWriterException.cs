using System;
using System.Collections.Generic;
using System.Text;

namespace Essential.Diagnostics
{
    [Serializable()]
    public class NetworkTextWriterException : Exception
    {
        public NetworkTextWriterException() : base() { }
        public NetworkTextWriterException(string message) : base(message) { }
        public NetworkTextWriterException(string message, System.Exception inner) : base(message, inner) { }
        protected NetworkTextWriterException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) { }
    }
}
