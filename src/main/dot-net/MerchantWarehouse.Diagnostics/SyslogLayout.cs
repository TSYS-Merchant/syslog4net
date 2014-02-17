using System.IO;
using log4net.Core;
using log4net.Layout;
using MerchantWarehouse.Diagnostics.Converters;
using System.Text;

namespace MerchantWarehouse.Diagnostics
{
    /// <summary>
    /// Log4net layout class with default support for the Syslog message format as described in the TOPS Syslong standard: https://confluence.mw.inc/display/TO/TOps+Syslog+Standard
    /// </summary>
    internal class SyslogLayout : LayoutSkeleton
    {
        private readonly PatternLayout _layout;

        // http://tools.ietf.org/html/rfc5424#section-6.1
        private const int SyslogMaxMessageLength = 2048;

        /// <summary>
        /// Instantiates a new instance of <see cref="SyslogLayout"/>
        /// </summary>
        public SyslogLayout()
        {
            IgnoresException = false;  //TODO deal with this. sealed?

            this._layout = new PatternLayout("<%mw-priority>1 %utcdate{yyyy-MM-ddTHH:mm:ss:FFZ} %mw-hostname %appdomain %mw-process-id %mw-message-id %mw-structured-data %message%newline");

            this._layout.AddConverter("mw-priority", typeof(PriorityConverter));
            this._layout.AddConverter("mw-hostname", typeof(HostnameConverter));
            //_layout.AddConverter("mw-app-domain", typeof(ApplicationNameConverter)); //use %appdomain, same implmentation
            this._layout.AddConverter("mw-process-id", typeof(ProcessIdConverter));
            this._layout.AddConverter("mw-message-id", typeof(MessageIdConverter));
            this._layout.AddConverter("mw-structured-data", typeof(StructuredDataConverter));
            this._layout.AddConverter("mw-thread-id", typeof(ThreadIdConverter));
            this._layout.AddConverter("mw-thread-name", typeof(ThreadNameConverter));
            this._layout.AddConverter("mw-command-line", typeof(CommandLineConverter));
            this._layout.AddConverter("mw-process-name", typeof(ProcessNameConverter));

            this.ActivateOptions();
        }

        /// <summary>
        /// Formats data within the event and writes the formatted data out to the provided writer instance
        /// </summary>
        /// <param name="writer">writer to output the formatted data to</param>
        /// <param name="logEvent">logging event data to use</param>
        override public void Format(TextWriter writer, LoggingEvent logEvent)
        {
            using (var stringWriter = new StringWriter())
            {
                this._layout.Format(stringWriter, logEvent);

                // truncate the message to SYSLOG_MAX_MESSAGE_LENGTH or fewer bytes
                string message = stringWriter.ToString();

                var utf8 = Encoding.UTF8;

                byte[] utfBytes = utf8.GetBytes(message);
                if (utfBytes.Length > SyslogMaxMessageLength)
                {
                    message = utf8.GetString(utfBytes, 0, SyslogMaxMessageLength);
                }

                writer.Write(message);
            }
        }

        /// <summary>
        /// Activates the use of options for the converter allowing the underlying PatternLayout implmentation to behave correctly
        /// </summary>
        override public void ActivateOptions()
        {
            this._layout.ActivateOptions();
        }
    }
}
