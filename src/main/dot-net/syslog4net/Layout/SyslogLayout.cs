using System.IO;
using log4net.Core;
using log4net.Layout;
using System.Text;
using System;
using System.Globalization;

namespace syslog4net.Layout
{
    using syslog4net.Converters;

    /// <summary>
    /// Log4net layout class with default support for the Syslog message format as described in Syslog IETF 5424 standard: http://tools.ietf.org/html/rfc5424
    /// </summary>
    public class SyslogLayout : LayoutSkeleton
    {
        private readonly PatternLayout _layout;

        // http://tools.ietf.org/html/rfc5424#section-6.1
        private const int SyslogMaxMessageLength = 2048;
        private const int SyslogFacilityCodeDefault = 16; // Local0

        /// <summary>
        /// Instantiates a new instance of <see cref="SyslogLayout"/>
        /// </summary>
        public SyslogLayout()
        {
            IgnoresException = false;  //TODO deal with this. sealed?

            this._layout = new PatternLayout("<%syslog-priority>1 %utcdate{yyyy-MM-ddTHH:mm:ss.FFZ} %syslog-hostname %appdomain"
                + " %syslog-process-id %syslog-message-id %syslog-structured-data %message%newline%exception");

            this._layout.AddConverter("syslog-priority", typeof(PriorityConverter));
            this._layout.AddConverter("syslog-hostname", typeof(HostnameConverter));
            this._layout.AddConverter("syslog-process-id", typeof(ProcessIdConverter));
            this._layout.AddConverter("syslog-message-id", typeof(MessageIdConverter));
            this._layout.AddConverter("syslog-structured-data", typeof(StructuredDataConverter));
            this._layout.AddConverter("appdomain", typeof(ProcessNameConverter));
        }

        /// <summary>
        /// Formats data within the event and writes the formatted data out to the provided writer instance
        /// </summary>
        /// <param name="writer">writer to output the formatted data to</param>
        /// <param name="logEvent">logging event data to use</param>
        public override void Format(TextWriter writer, LoggingEvent logEvent)
        {
            logEvent.Properties["log4net:StructuredDataPrefix"] = StructuredDataPrefix;
            logEvent.Properties["log4net:FacilityCode"] = FacilityCode;

            using (var stringWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                this._layout.Format(stringWriter, logEvent);

                // truncate the message to SYSLOG_MAX_MESSAGE_LENGTH or fewer bytes
                string message = stringWriter.ToString();
                if(bool.Parse(PrependMessageLength))
                    message = message.Length.ToString() + " " + message;
                
                int lMaxMessageLength = Convert.ToInt32(this.MaxMessageLength);
                if (message.Length > lMaxMessageLength)
                {
                    message = message.Substring(0, lMaxMessageLength);
                }

                writer.Write(message);
            }
        }

        /// <summary>
        /// Sets the syslog structured data ID. See http://tools.ietf.org/html/rfc5424#section-6.3.2 for more details.
        /// </summary>
        public string StructuredDataPrefix { get; set; }
        public string MaxMessageLength { get; set; }
        public string FacilityCode { get; set; }
        public string PrependMessageLength { get; set; }

        /// <summary>
        /// Activates the use of options for the converter allowing the underlying PatternLayout implmentation to behave correctly
        /// </summary>
        public override void ActivateOptions()
        {
            if (string.IsNullOrEmpty(this.StructuredDataPrefix))
            {
                throw new ArgumentNullException("StructuredDataPrefix");
            }
            if (string.IsNullOrEmpty(this.MaxMessageLength))
            {
                this.MaxMessageLength = Convert.ToString(SyslogMaxMessageLength);
            }
            else 
            {
                this.MaxMessageLength = Convert.ToInt32(this.MaxMessageLength).ToString();
            }

            if (string.IsNullOrEmpty(this.FacilityCode))
            {
                this.FacilityCode = Convert.ToString(SyslogFacilityCodeDefault);
            }

            if (string.IsNullOrEmpty(this.PrependMessageLength))
            {
                this.PrependMessageLength = false.ToString();
            }

            this._layout.ActivateOptions();
        }
    }
}
