using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Essential.Diagnostics
{
    public interface INetworkTextWriter : IDisposable
    {
        void Write(TraceEventCache eventCache, string value);
        void WriteLine(TraceEventCache eventCache, string value);
    }
}
