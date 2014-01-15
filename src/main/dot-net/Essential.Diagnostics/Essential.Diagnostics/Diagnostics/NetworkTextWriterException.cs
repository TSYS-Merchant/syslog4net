//// Orignal Class Added to Essentials.Diagnostics - 1/14/2014 - Copyright © 2014 Merchant Warehouse
//// All Code Released Under the MS-PL License: http://opensource.org/licenses/MS-PL

using System;
using System.Collections.Generic;
using System.Text;

namespace Essential.Diagnostics
{
    /// <summary>
    /// Exception class surfaced for any issues when writing text to a configured network
    /// </summary>
    [Serializable()]
    public class NetworkTextWriterException : Exception
    {
        /// <summary>
        /// Creates a new instance of <see cref="NetworkTextWriterException"/>
        /// </summary>
        public NetworkTextWriterException() : base() { }

        /// <summary>
        /// Creates a new instance of <see cref="NetworkTextWriterException"/>
        /// </summary>
        /// <param name="message">Message to attach to the exception</param>
        public NetworkTextWriterException(string message) : base(message) { }

        /// <summary>
        /// Creates a new instance of <see cref="NetworkTextWriterException"/>
        /// </summary>
        /// <param name="message">Message to attach to the exception</param>
        /// <param name="inner">Inner-exception that bubbled up from lower layers in the stack</param>
        public NetworkTextWriterException(string message, System.Exception inner) : base(message, inner) { }

        /// <summary>
        /// Creates a new instance of <see cref="NetworkTextWriterException"/>
        /// </summary>
        /// <param name="info">serilization data</param>
        /// <param name="context">streaming context manager</param>
        protected NetworkTextWriterException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
