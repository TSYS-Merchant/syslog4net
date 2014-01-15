//// Orignal Class Added to Essentials.Diagnostics - 1/14/2014 - Copyright © 2014 Merchant Warehouse
//// All Code Released Under the MS-PL License: http://opensource.org/licenses/MS-PL

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Essential.Diagnostics
{
    /// <summary>
    /// Provides logging capibilities for network connections. Currently only supports TCP and UDP, designed to support any protocol in <see cref="ProtocolType"/>
    /// </summary>
    public class NetworkTraceListener : TraceListenerBase
    {
        private const string _defaultTemplate = "{UtcDateTime:yyyy-MM-ddTHH:mm:ss.fffZ} {MachineName} {Source} {ProcessId}";
        private IPAddress _remoteAddress;
        private int _remotePort;
        private int _localPort;
        private Encoding _encoding;
        private INetworkTextWriter _writer;

        // I would rather leave this out of the IMPL but I cannot see a clean way to write to 2 sources under different 
        // circumstaces with a single source string without embedding another trace source call somewhere. 
        private TraceSource _exceptionSource;

        private static string[] _supportedAttributes = new string[] 
            { 
                "template", "Template", 
                "convertWriteToEvent", "ConvertWriteToEvent",
                "remoteAddress", "RemoteAddress",
                "remotePort", "RemotePort",
                "protocol", "Protocol",
                "textEncoding", "TextEncoding",
                "nextSource", "NextSource"
            };

        TraceFormatter traceFormatter = new TraceFormatter();

        /// <summary>
        /// Constructor. Writes to a rolling text file using the default name.
        /// </summary>
        public NetworkTraceListener()
        {
        }


        /// <summary>
        /// Gets or sets the token format string to use to display trace output.
        /// </summary>
        /// <remarks>
        /// <para>
        /// See TraceFormatter for details of the supported formats.
        /// </para>
        /// <para>
        /// The default value is "{DateTime:u} [{Thread}] {EventType} {Source} {Id}: {Message}{Data}".
        /// </para>
        /// <para>
        /// To get a format that matches Microsoft.VisualBasic.Logging.FileLogTraceListener, 
        /// use the tab delimited format "{Source}&#x9;{EventType}&#x9;{Id}&#x9;{Message}{Data}". 
        /// (Note: In the config XML file the TAB characters are XML encoded; if specifying 
        /// in C# code the delimiters  would be '\t'.)
        /// </para>
        /// <para>
        /// To get a format matching FileLogTraceListener with all TraceOutputOptions enabled, use
        /// "{Source}&#x9;{EventType}&#x9;{Id}&#x9;{Message}&#x9;{Callstack}&#x9;{LogicalOperationStack}&#x9;{DateTime:u}&#x9;{ProcessId}&#x9;{ThreadId}&#x9;{Timestamp}&#x9;{MachineName}".
        /// </para>
        /// <para>
        /// To get a format simliar to that produced by System.Diagnostics.TextWriterTraceListener,
        /// use "{Source} {EventType}: {Id} : {Message}{Data}".
        /// </para>
        /// </remarks>
        public string Template
        {
            get
            {
                // Default format matches Microsoft.VisualBasic.Logging.FileLogTraceListener
                if (Attributes.ContainsKey("template"))
                {
                    return Attributes["template"];
                }
                else
                {
                    return _defaultTemplate;
                }
            }
            set
            {
                Attributes["template"] = value;
            }
        }

        /// <summary>
        /// Gets or sets whether calls to the Trace class static Write and WriteLine methods should be converted to Verbose events,
        /// and then filtered and formatted (otherwise they are output directly to the file).
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Boolean.TryParse(System.String,System.Boolean@)", Justification = "Default value is acceptable if conversion fails.")]
        public bool ConvertWriteToEvent
        {
            get
            {
                // Default behaviour is to convert Write to event.
                var convertWriteToEvent = true;
                if (Attributes.ContainsKey("convertWriteToEvent"))
                {
                    bool.TryParse(Attributes["convertWriteToEvent"], out convertWriteToEvent);
                }
                return convertWriteToEvent;
            }
            set
            {
                Attributes["convertWriteToEvent"] = value.ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Gets or sets the configured IP address where messages will be targeted.
        /// </summary>
        public IPAddress RemoteAddress
        {
            get
            {
                if (Attributes.ContainsKey("remoteAddress"))
                {
                    return IPAddress.Parse(Attributes["remoteAddress"]);
                }

                return null;
            }
            set
            {
                Attributes["remoteAddress"] = value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the port where messages will be targeted
        /// </summary>
        public int RemotePort
        {
            get
            {
                if (Attributes.ContainsKey("remotePort"))
                {
                    return int.Parse(Attributes["remotePort"]);
                }

                return -1;
            }
            set
            {
                Attributes["remotePort"] = value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ProtocolType"/>. Currently Supports TCP|UDP - Default is UDP
        /// </summary>
        public ProtocolType Protocol
        {
            get
            {
                if (Attributes.ContainsKey("protocol"))
                {
                    switch (Attributes["protocol"])
                    {
                        case "UDP":
                            return ProtocolType.Udp;
                        case "TCP":
                            return ProtocolType.Tcp;
                        default:
                            return ProtocolType.Udp;
                    }
                }

                return ProtocolType.Udp;
            }
            set
            {
                Attributes["protocol"] = value.ToString().ToUpper();
            }
        }

        /// <summary>
        /// Gets or sets the text encoding to use. All types listed in <see cref="Encoding"/> are supported
        /// </summary>
        public Encoding TextEncoding
        {
            get
            {
                if (Attributes.ContainsKey("encoding"))
                {
                    switch (Attributes["encoding"])
                    {
                        case "ASCII":
                            return Encoding.ASCII;
                        case "BigEndianUnicode":
                            return Encoding.BigEndianUnicode;
                        case "Default":
                            return Encoding.Default;
                        case "Unicode":
                            return Encoding.Unicode;
                        case "UTF32":
                            return Encoding.UTF32;
                        case "UTF7":
                            return Encoding.UTF7;
                        case "UTF8":
                            return Encoding.UTF8;
                        default:
                            return Encoding.Default;
                    }
                }

                return Encoding.Default;
            }
            set
            {
                Attributes["encoding"] = value.EncodingName;
            }
        }

        /// <summary>
        /// Gets or sets the name of the next source to process the event
        /// </summary>
        /// <remarks>
        /// Allows for chaining of sources with a single TraceSource instance. Value should be the name of another configured source described in the App.config or Web.config.
        /// WARNING: Does not check for circular references. Take care when configuring this property to prevent stack overflow crashes of the application. 
        /// </remarks>
        public string NextSource
        {
            get
            {
                if (Attributes.ContainsKey("nextSource"))
                {
                    return Attributes["nextSource"];
                }

                return null;
            }
            set
            {
                Attributes["nextSource"] = value.ToString();
            }
        }

        /// <summary>
        /// Gets whether the listener internally handles thread safety
        /// (or if the System.Diagnostics framework needs to co-ordinate threading).
        /// </summary>
        public override bool IsThreadSafe
        {
            get { return true; }
        }

        /// <summary>
        /// Gets or sets configured endpoint instance to be used by lower level <see cref="INetworkTextWriter"/> instances
        /// </summary>
        protected IPEndPoint RemoteEndpoint { get; set; }

        /// <summary>
        /// Allowed attributes for this trace listener.
        /// </summary>
        protected override string[] GetSupportedAttributes()
        {
            return _supportedAttributes;
        }

        /// <summary>
        /// Handles trace Write calls - either output directly to console or convert to verbose event
        /// based on the ConvertWriteToEvent setting.
        /// </summary>
        protected override void Write(string category, string message, object data)
        {
            CreateWriter();
            // Either format as event or write direct to stream based on flag.
            if (ConvertWriteToEvent)
            {
                base.Write(category, message, data);
            }
            else
            {
                _writer.Write(null, message);
            }
        }

        /// <summary>
        /// Handles trace Write calls - either output directly to console or convert to verbose event
        /// based on the ConvertWriteToEvent setting.
        /// </summary>
        protected override void WriteLine(string category, string message, object data)
        {
            CreateWriter();
            // Either format as event or write direct to stream based on flag.
            if (ConvertWriteToEvent)
            {
                base.WriteLine(category, message, data);
            }
            else
            {
                _writer.WriteLine(null, message);
            }
        }

        /// <summary>
        /// Write trace event with data.
        /// </summary>
        protected override void WriteTrace(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message, Guid? relatedActivityId, object[] data)
        {
            CreateWriter();

            var output = traceFormatter.Format(Template,
                this,
                eventCache,
                source,
                eventType,
                id,
                message,
                relatedActivityId,
                data
                );
            _writer.WriteLine(eventCache, output);

            // TODO: Make configurable?
            if (eventType == TraceEventType.Critical
                || eventType == TraceEventType.Error
                || eventType == TraceEventType.Warning)
            {
                _exceptionSource.TraceEvent(eventType, id, message, data);
            }
        }

        private void CreateWriter()
        {
            if (_writer == null)
            {
                RemoteEndpoint = new IPEndPoint(RemoteAddress, RemotePort);

                var textWriter = new NetworkTextWriter(RemoteEndpoint, TextEncoding);
                _writer = textWriter.Create(Protocol);
            }

            if (_exceptionSource == null && !string.IsNullOrEmpty(NextSource))
            {
                _exceptionSource = new TraceSource(NextSource);
            }
        }
    }
}
