//// Orignal Class Added to Essentials.Diagnostics - 1/14/2014 - Copyright © 2014 Merchant Warehouse
//// All Code Released Under the MS-PL License: http://opensource.org/licenses/MS-PL

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Essential.Diagnostics
{
    /// <summary>
    /// Interface for implmentations that provide UTF-8 Text writing capibilities via the method implemented by the writer class.
    /// </summary>
    public interface INetworkTextWriter : IDisposable
    {
        /// <summary>
        /// Writes a message to the wire as configured
        /// </summary>
        /// <param name="eventCache">logging event context data</param>
        /// <param name="value">value to output to the file</param>
        void Write(TraceEventCache eventCache, string value);

        /// <summary>
        /// Writes a message with an appended carriage return to the wire as configured
        /// </summary>
        /// <param name="eventCache">logging event context data</param>
        /// <param name="value">value to output to the file</param>
        void WriteLine(TraceEventCache eventCache, string value);
    }
}
